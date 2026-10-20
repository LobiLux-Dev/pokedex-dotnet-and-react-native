namespace Pokedex.Domain;

/// <summary>
/// Species level information (the "meta" data of a Pokémon).
/// </summary>
public sealed record PokemonSpecies(
    string Name,
    string? Genus,
    string? Description,
    string? Habitat,
    string? Color,
    int? EvolutionChainId,
    bool IsLegendary,
    bool IsMythical);
