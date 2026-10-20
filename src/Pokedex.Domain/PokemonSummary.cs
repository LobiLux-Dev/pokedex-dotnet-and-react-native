namespace Pokedex.Domain;

/// <summary>
/// Lightweight representation of a Pokémon, used for list views and filtering.
/// </summary>
public sealed record PokemonSummary(
    int Id,
    string Name,
    IReadOnlyList<string> Types,
    string SpriteUrl,
    string ArtworkUrl);
