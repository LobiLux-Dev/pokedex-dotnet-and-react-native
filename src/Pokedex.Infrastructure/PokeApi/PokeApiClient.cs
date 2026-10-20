using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pokedex.Application.Abstractions;
using Pokedex.Application.Common;
using Pokedex.Domain;
using Pokedex.Infrastructure.Options;
using Pokedex.Infrastructure.PokeApi.Contracts;

namespace Pokedex.Infrastructure.PokeApi;

/// <summary>
/// Reads Pokémon data from PokeAPI, mapping the upstream payloads to the domain model
/// and caching every expensive call.
/// </summary>
public sealed class PokeApiClient : IPokemonRepository
{
    private const string CatalogCacheKey = "pokeapi:catalog";
    private const string TypeIndexCacheKey = "pokeapi:type-index";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    private readonly HttpClient _httpClient;
    private readonly HybridCache _cache;
    private readonly PokeApiOptions _options;
    private readonly ILogger<PokeApiClient> _logger;

    public PokeApiClient(
        HttpClient httpClient,
        HybridCache cache,
        IOptions<PokeApiOptions> options,
        ILogger<PokeApiClient> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PokemonSummary>> GetCatalogAsync(CancellationToken cancellationToken)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = _options.CatalogCacheDuration
        };

        var result = await _cache.GetOrCreateAsync<PokeApiResult<IReadOnlyList<PokemonSummary>>>(
            CatalogCacheKey,
            async ct =>
            {
                var list = await GetJsonAsync<PokeApiNamedResourceList>(
                    "pokemon?offset=0&limit=100000",
                    ct);

                if (list is null || list.Results.Count == 0)
                {
                    _logger.LogWarning("PokeAPI returned an empty Pokémon catalogue.");
                    return new PokeApiResult<IReadOnlyList<PokemonSummary>>([]);
                }

                var typeIndex = await GetTypeIndexAsync(ct);

                _logger.LogDebug(
                    "Type index ready: {TypeCount} types, {MappedCount} Pokémon mapped.",
                    typeIndex.Types.Count,
                    typeIndex.TypesByPokemon.Count);

                var summaries = new List<PokemonSummary>(list.Results.Count);
                foreach (var resource in list.Results)
                {
                    var id = TryParseId(resource.Url);
                    if (id is null)
                    {
                        continue;
                    }

                    var types = typeIndex.TypesByPokemon.TryGetValue(resource.Name, out var pokemonTypes)
                        ? pokemonTypes
                        : [];

                    summaries.Add(new PokemonSummary(
                        id.Value,
                        resource.Name,
                        types,
                        $"{_options.SpriteBaseUrl}/{id.Value}.png",
                        $"{_options.ArtworkBaseUrl}/{id.Value}.png"));
                }

                _logger.LogInformation("Pokémon catalogue built with {Count} entries.", summaries.Count);
                return new PokeApiResult<IReadOnlyList<PokemonSummary>>(summaries);
            },
            entryOptions);

        return result.Value ?? [];
    }

    public async Task<Pokemon?> GetPokemonAsync(string nameOrId, CancellationToken cancellationToken)
    {
        var trimmed = nameOrId.Trim();
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = _options.DetailCacheDuration
        };

        var result = await _cache.GetOrCreateAsync<PokeApiResult<Pokemon>>(
            $"pokeapi:pokemon:{trimmed.ToLowerInvariant()}",
            async ct =>
            {
                var raw = await GetJsonAsync<PokeApiPokemon>(
                    $"pokemon/{Uri.EscapeDataString(trimmed)}",
                    ct);

                if (raw is null)
                {
                    return new PokeApiResult<Pokemon>(null);
                }

                var moves = await EnrichMovesAsync(MapMoves(raw.Moves), ct);
                return new PokeApiResult<Pokemon>(MapPokemon(raw, moves));
            },
            entryOptions);

        return result.Value;
    }

    public async Task<PokemonSpecies?> GetSpeciesAsync(string nameOrId, CancellationToken cancellationToken)
    {
        var trimmed = nameOrId.Trim();
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = _options.DetailCacheDuration
        };

        var result = await _cache.GetOrCreateAsync<PokeApiResult<PokemonSpecies>>(
            $"pokeapi:species:{trimmed.ToLowerInvariant()}",
            async ct =>
            {
                var raw = await GetJsonAsync<PokeApiSpecies>(
                    $"pokemon-species/{Uri.EscapeDataString(trimmed)}",
                    ct);

                return new PokeApiResult<PokemonSpecies>(raw is null ? null : MapSpecies(raw));
            },
            entryOptions);

        return result.Value;
    }

    public async Task<EvolutionChain?> GetEvolutionChainAsync(int chainId, CancellationToken cancellationToken)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = _options.DetailCacheDuration
        };

        var result = await _cache.GetOrCreateAsync<PokeApiResult<EvolutionChain>>(
            $"pokeapi:evolution-chain:{chainId}",
            async ct =>
            {
                var raw = await GetJsonAsync<PokeApiEvolutionChain>(
                    $"evolution-chain/{chainId.ToString(CultureInfo.InvariantCulture)}",
                    ct);

                if (raw?.Chain is null)
                {
                    return new PokeApiResult<EvolutionChain>(null);
                }

                return new PokeApiResult<EvolutionChain>(new EvolutionChain(raw.Id, MapNode(raw.Chain)));
            },
            entryOptions);

        return result.Value;
    }

    public async Task<IReadOnlyList<PokemonTypeInfo>> GetTypesAsync(CancellationToken cancellationToken)
    {
        var index = await GetTypeIndexAsync(cancellationToken);
        return index.Types;
    }

    public async Task<PokemonMoveDetails?> GetMoveDetailsAsync(string moveName, CancellationToken cancellationToken)
    {
        var trimmed = moveName.Trim();
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = _options.MoveCacheDuration
        };

        var result = await _cache.GetOrCreateAsync<PokeApiResult<PokemonMoveDetails>>(
            $"pokeapi:move:{trimmed.ToLowerInvariant()}",
            async ct =>
            {
                var raw = await GetJsonAsync<PokeApiMove>(
                    $"move/{Uri.EscapeDataString(trimmed)}",
                    ct);

                if (raw is null)
                {
                    return new PokeApiResult<PokemonMoveDetails>(null);
                }

                return new PokeApiResult<PokemonMoveDetails>(new PokemonMoveDetails(
                    raw.Name,
                    raw.Type?.Name,
                    raw.DamageClass?.Name,
                    raw.Power,
                    raw.Accuracy));
            },
            entryOptions);

        return result.Value;
    }

    private async Task<PokemonTypeIndex> GetTypeIndexAsync(CancellationToken cancellationToken)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = _options.CatalogCacheDuration
        };

        var result = await _cache.GetOrCreateAsync<TypeIndexResult>(
            TypeIndexCacheKey,
            async ct =>
            {
                var list = await GetJsonAsync<PokeApiNamedResourceList>("type", ct)
                    ?? new PokeApiNamedResourceList();

                var details = await FetchBoundedAsync(
                    list.Results,
                    async resource =>
                    {
                        try
                        {
                            return await GetJsonAsync<PokeApiTypeDetail>(
                                $"type/{Uri.EscapeDataString(resource.Name)}",
                                ct);
                        }
                        catch (ExternalServiceException ex)
                        {
                            _logger.LogWarning(ex, "Could not load the type '{Type}'.", resource.Name);
                            return null;
                        }
                    },
                    ct);

                var types = new List<PokemonTypeInfo>();
                var typesByPokemon = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var detail in details)
                {
                    if (detail is null)
                    {
                        continue;
                    }

                    types.Add(new PokemonTypeInfo(detail.Name, detail.Pokemon.Count));

                    foreach (var entry in detail.Pokemon)
                    {
                        var pokemonName = entry.Pokemon?.Name;
                        if (string.IsNullOrWhiteSpace(pokemonName))
                        {
                            continue;
                        }

                        if (!typesByPokemon.TryGetValue(pokemonName, out var listForPokemon))
                        {
                            listForPokemon = [];
                            typesByPokemon[pokemonName] = listForPokemon;
                        }

                        listForPokemon.Add(detail.Name);
                    }
                }

                var readOnlyMap = typesByPokemon.ToDictionary(
                    pair => pair.Key,
                    pair => (IReadOnlyList<string>)pair.Value
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase);

                return new TypeIndexResult(new PokemonTypeIndex(types, readOnlyMap));
            },
            entryOptions);

        return result.Index;
    }

    private async Task<IReadOnlyList<PokemonMove>> EnrichMovesAsync(
        IReadOnlyList<PokemonMove> moves,
        CancellationToken cancellationToken)
    {
        var levelUpNames = moves
            .Where(move => move.LearnMethod == "level-up")
            .Select(move => move.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (levelUpNames.Count == 0)
        {
            return moves;
        }

        var details = await FetchBoundedAsync(
            levelUpNames,
            async name => (Name: name, Details: await GetMoveDetailsAsync(name, cancellationToken)),
            cancellationToken);

        var lookup = details
            .Where(item => item.Details is not null)
            .ToDictionary(item => item.Name, item => item.Details!, StringComparer.OrdinalIgnoreCase);

        return moves
            .Select(move => lookup.TryGetValue(move.Name, out var detailsForMove)
                ? move with
                {
                    Type = detailsForMove.Type,
                    DamageClass = detailsForMove.DamageClass,
                    Power = detailsForMove.Power,
                    Accuracy = detailsForMove.Accuracy
                }
                : move)
            .ToList();
    }

    private async Task<T?> GetJsonAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(path, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ExternalServiceException(
                $"PokeAPI responded with {(int)response.StatusCode} for '{path}': {Truncate(body)}");
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        }
        catch (JsonException ex)
        {
            throw new ExternalServiceException($"PokeAPI returned an invalid payload for '{path}'.", ex);
        }
    }

    /// <summary>
    /// Sends the request and turns the transport and resilience failures (timeouts, broken
    /// circuits, DNS errors) into <see cref="ExternalServiceException"/> so the API answers
    /// with a 502 instead of a 500.
    /// </summary>
    private async Task<HttpResponseMessage> SendAsync(string path, CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.GetAsync(path, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The client went away, nothing to report.
            throw;
        }
        catch (Exception exception)
        {
            throw new ExternalServiceException($"PokeAPI request '{path}' failed.", exception);
        }
    }

    private async Task<IReadOnlyList<TResult>> FetchBoundedAsync<TSource, TResult>(
        IReadOnlyList<TSource> sources,
        Func<TSource, Task<TResult>> factory,
        CancellationToken cancellationToken)
    {
        var results = new TResult[sources.Count];
        using var semaphore = new SemaphoreSlim(Math.Max(1, _options.MaxConcurrency));

        var tasks = sources.Select(async (source, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                results[index] = await factory(source);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return results;
    }

    private static Pokemon MapPokemon(PokeApiPokemon raw, IReadOnlyList<PokemonMove> moves)
    {
        var types = raw.Types
            .Where(slot => slot.Type is not null)
            .OrderBy(slot => slot.Slot)
            .Select(slot => new PokemonType(slot.Type!.Name))
            .ToList();

        var abilities = raw.Abilities
            .Where(slot => slot.Ability is not null)
            .OrderBy(slot => slot.Slot)
            .Select(slot => new PokemonAbility(slot.Ability!.Name, slot.IsHidden))
            .ToList();

        var stats = raw.Stats
            .Where(entry => entry.Stat is not null)
            .Select(entry => new PokemonStat(entry.Stat!.Name, entry.BaseStat, entry.Effort))
            .ToList();

        var sprites = new PokemonSprites(
            raw.Sprites?.FrontDefault,
            raw.Sprites?.FrontShiny,
            raw.Sprites?.BackDefault,
            raw.Sprites?.BackShiny,
            raw.Sprites?.FrontFemale,
            raw.Sprites?.FrontShinyFemale);

        return new Pokemon(
            raw.Id,
            raw.Name,
            raw.Height,
            raw.Weight,
            raw.BaseExperience,
            types,
            abilities,
            stats,
            sprites,
            raw.Species?.Name ?? raw.Name,
            moves);
    }

    private static List<PokemonMove> MapMoves(IReadOnlyList<PokeApiMoveEntry> entries)
    {
        var grouped = new Dictionary<string, List<PokemonMove>>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in entries)
        {
            var name = entry.Move?.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (!grouped.TryGetValue(name, out var moves))
            {
                moves = [];
                grouped[name] = moves;
            }

            if (entry.VersionGroupDetails.Count == 0)
            {
                moves.Add(new PokemonMove(name, null, null, null, null, 0, "unknown", "unknown"));
                continue;
            }

            foreach (var detail in entry.VersionGroupDetails)
            {
                moves.Add(new PokemonMove(
                    name,
                    null,
                    null,
                    null,
                    null,
                    detail.LevelLearnedAt,
                    detail.MoveLearnMethod?.Name ?? "unknown",
                    detail.VersionGroup?.Name ?? "unknown"));
            }
        }

        return grouped.Values.Select(KeepRelevantEntries).ToList();
    }

    /// <summary>
    /// PokeAPI returns a move once per version group. Only the most relevant entry per move is kept:
    /// the lowest level for level-up moves, and one entry per remaining learn method.
    /// </summary>
    private static PokemonMove KeepRelevantEntries(List<PokemonMove> moves)
    {
        var levelUp = moves
            .Where(move => move.LearnMethod == "level-up")
            .OrderBy(move => move.Level)
            .FirstOrDefault();

        if (levelUp is not null)
        {
            return levelUp;
        }

        return moves
            .GroupBy(move => move.LearnMethod, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderBy(move => move.Level).First())
            .OrderBy(move => move.LearnMethod, StringComparer.Ordinal)
            .First();
    }

    private static PokemonSpecies MapSpecies(PokeApiSpecies raw)
    {
        var genus = raw.Genera?
            .FirstOrDefault(entry => string.Equals(entry.Language?.Name, "es", StringComparison.OrdinalIgnoreCase))?.Genus;

        genus ??= raw.Genus;

        var description = PreferLanguage(raw.FlavorTextEntries, "es")
            ?? PreferLanguage(raw.FlavorTextEntries, "en");

        var chainId = TryParseId(raw.EvolutionChain?.Url)
            ?? (int.TryParse(raw.EvolutionChain?.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : null);

        return new PokemonSpecies(
            raw.Name,
            CleanText(genus),
            description,
            raw.Habitat?.Name,
            raw.Color?.Name,
            chainId,
            raw.IsLegendary,
            raw.IsMythical);
    }

    private static EvolutionNode MapNode(PokeApiEvolutionNode node) => new(
        node.Species?.Name ?? "unknown",
        node.EvolutionDetails.Select(detail => new EvolutionDetail(
            detail.Trigger?.Name,
            detail.MinLevel,
            detail.Item?.Name,
            detail.HeldItem?.Name,
            detail.KnownMove?.Name,
            string.IsNullOrWhiteSpace(detail.TimeOfDay) ? null : detail.TimeOfDay,
            detail.TurnUpsideDown,
            detail.Gender)).ToList(),
        node.EvolvesTo.Select(MapNode).ToList());

    private static string? PreferLanguage(IReadOnlyList<PokeApiFlavorTextEntry> entries, string language) =>
        entries
            .Where(entry => string.Equals(entry.Language?.Name, language, StringComparison.OrdinalIgnoreCase))
            .Select(entry => CleanText(entry.FlavorText))
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

    private static string? CleanText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var cleaned = text
            .Replace("\n", " ")
            .Replace("\f", " ")
            .Replace("♀", string.Empty)
            .Replace("♂", string.Empty);

        cleaned = WhitespaceRegex.Replace(cleaned, " ").Trim();
        return cleaned.Length == 0 ? null : cleaned;
    }

    private static int? TryParseId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var segments = url.TrimEnd('/').Split('/');
        var last = segments[^1];
        return int.TryParse(last, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;
    }

    private static string Truncate(string value) =>
        value.Length <= 200 ? value : value[..200];

    private sealed record PokeApiResult<T>(T? Value);

    private sealed record TypeIndexResult(PokemonTypeIndex Index);

    private sealed record PokemonTypeIndex(
        IReadOnlyList<PokemonTypeInfo> Types,
        IReadOnlyDictionary<string, IReadOnlyList<string>> TypesByPokemon);
}
