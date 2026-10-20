using Pokedex.Application.Abstractions;
using Pokedex.Domain;

namespace Pokedex.Tests;

/// <summary>
/// In-memory repository used by the tests so they never touch the network.
/// </summary>
public sealed class FakePokemonRepository : IPokemonRepository
{
    public List<PokemonSummary> Catalog { get; } = [];

    public Dictionary<string, Pokemon> Pokemon { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, PokemonSpecies> Species { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<int, EvolutionChain> EvolutionChains { get; } = [];

    public List<PokemonTypeInfo> Types { get; } = [];

    public Dictionary<string, PokemonMoveDetails> Moves { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Task<IReadOnlyList<PokemonSummary>> GetCatalogAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<PokemonSummary>>(Catalog);

    public Task<Pokemon?> GetPokemonAsync(string nameOrId, CancellationToken cancellationToken) =>
        Task.FromResult(Pokemon.GetValueOrDefault(nameOrId));

    public Task<PokemonSpecies?> GetSpeciesAsync(string nameOrId, CancellationToken cancellationToken) =>
        Task.FromResult(Species.GetValueOrDefault(nameOrId));

    public Task<EvolutionChain?> GetEvolutionChainAsync(int chainId, CancellationToken cancellationToken) =>
        Task.FromResult(EvolutionChains.GetValueOrDefault(chainId));

    public Task<IReadOnlyList<PokemonTypeInfo>> GetTypesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<PokemonTypeInfo>>(Types);

    public Task<PokemonMoveDetails?> GetMoveDetailsAsync(string moveName, CancellationToken cancellationToken) =>
        Task.FromResult(Moves.GetValueOrDefault(moveName));
}
