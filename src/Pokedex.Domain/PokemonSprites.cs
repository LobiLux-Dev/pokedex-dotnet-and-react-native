namespace Pokedex.Domain;

/// <summary>
/// Sprite URLs for a Pokémon. Any entry may be null when PokeAPI has no asset for it.
/// </summary>
public sealed record PokemonSprites(
    string? FrontDefault,
    string? FrontShiny,
    string? BackDefault,
    string? BackShiny,
    string? FrontFemale,
    string? FrontShinyFemale)
{
    public static readonly PokemonSprites Empty = new(null, null, null, null, null, null);
}
