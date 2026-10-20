namespace Pokedex.Application.Abstractions;

/// <summary>
/// Read access to the upstream Pokémon data.
/// Implemented in the infrastructure layer; the application layer stays unaware of HTTP details.
/// </summary>
public interface IPokemonRepository
{
    /// <summary>Full catalogue of Pokémon, used for listing, searching and filtering.</summary>
    Task<IReadOnlyList<Domain.PokemonSummary>> GetCatalogAsync(CancellationToken cancellationToken);

    /// <summary>Detailed information about a single Pokémon, or null when it does not exist.</summary>
    Task<Domain.Pokemon?> GetPokemonAsync(string nameOrId, CancellationToken cancellationToken);

    /// <summary>Species level information (genus, description, evolution chain id).</summary>
    Task<Domain.PokemonSpecies?> GetSpeciesAsync(string nameOrId, CancellationToken cancellationToken);

    /// <summary>Evolution chain by its numeric identifier.</summary>
    Task<Domain.EvolutionChain?> GetEvolutionChainAsync(int chainId, CancellationToken cancellationToken);

    /// <summary>Every Pokémon type together with the number of Pokémon that belong to it.</summary>
    Task<IReadOnlyList<Domain.PokemonTypeInfo>> GetTypesAsync(CancellationToken cancellationToken);

    /// <summary>Extra information about a move (type, power, accuracy).</summary>
    Task<Domain.PokemonMoveDetails?> GetMoveDetailsAsync(string moveName, CancellationToken cancellationToken);
}
