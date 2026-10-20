namespace Pokedex.Application.Dtos;

/// <summary>
/// Summary of a Pokémon, used for list views.
/// </summary>
public sealed record PokemonSummaryDto(
    int Id,
    string Name,
    string DisplayName,
    IReadOnlyList<string> Types,
    string SpriteUrl,
    string ArtworkUrl);
