using Microsoft.Extensions.Logging;
using Pokedex.Application.Abstractions;
using Pokedex.Application.Common;
using Pokedex.Application.Dtos;
using Pokedex.Application.Mapping;
using Pokedex.Domain;

namespace Pokedex.Application.Services;

/// <summary>
/// Implements the Pokémon use cases on top of <see cref="IPokemonRepository"/>.
/// </summary>
public sealed class PokemonService : IPokemonService
{
    private readonly IPokemonRepository _repository;
    private readonly ILogger<PokemonService> _logger;

    public PokemonService(IPokemonRepository repository, ILogger<PokemonService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<PokemonSummaryDto>> GetPokemonPageAsync(
        PokemonQuery query,
        CancellationToken cancellationToken)
    {
        var normalized = query.Normalize();
        var catalog = await _repository.GetCatalogAsync(cancellationToken);

        IEnumerable<PokemonSummary> filtered = catalog;

        if (normalized.Type is not null)
        {
            filtered = filtered.Where(pokemon =>
                pokemon.Types.Contains(normalized.Type, StringComparer.OrdinalIgnoreCase));
        }

        if (normalized.Search is not null)
        {
            filtered = filtered.Where(pokemon => Matches(pokemon, normalized.Search));
        }

        var ordered = filtered.OrderBy(pokemon => pokemon.Id).ToList();

        var items = ordered
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(PokemonMapper.ToSummaryDto)
            .ToList();

        return PagedResponse<PokemonSummaryDto>.Create(
            items,
            normalized.Page,
            normalized.PageSize,
            ordered.Count);
    }

    public async Task<PokemonDetailDto> GetPokemonDetailAsync(
        string nameOrId,
        CancellationToken cancellationToken)
    {
        var pokemon = await _repository.GetPokemonAsync(nameOrId, cancellationToken);
        if (pokemon is null)
        {
            throw new PokemonNotFoundException(nameOrId);
        }

        var species = await _repository.GetSpeciesAsync(pokemon.SpeciesName, cancellationToken);

        EvolutionChainDto? evolution = null;
        if (species?.EvolutionChainId is int chainId)
        {
            var chain = await _repository.GetEvolutionChainAsync(chainId, cancellationToken);
            if (chain is not null)
            {
                evolution = chain.ToEvolutionDto();
            }
        }

        var moves = pokemon.Moves.Select(PokemonMapper.ToMoveDto).ToList();

        return pokemon.ToDetailDto(species, evolution, moves);
    }

    public async Task<IReadOnlyList<PokemonTypeDto>> GetTypesAsync(CancellationToken cancellationToken)
    {
        var types = await _repository.GetTypesAsync(cancellationToken);
        return types.Select(PokemonMapper.ToTypeDto).ToList();
    }

    /// <summary>
    /// A search term matches when it is a prefix of the name or of the id,
    /// so "pika" finds "pikachu" and "25" finds #25.
    /// </summary>
    private static bool Matches(PokemonSummary pokemon, string search) =>
        pokemon.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase)
        || pokemon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)
            .StartsWith(search, StringComparison.Ordinal);
}
