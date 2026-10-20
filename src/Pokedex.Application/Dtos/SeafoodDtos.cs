namespace Pokedex.Application.Dtos;

/// <summary>
/// A seafood entry as shown in the guide list.
/// </summary>
public sealed record SeafoodSummaryDto(
    int Id,
    string Slug,
    string Title,
    string RealName,
    int PokemonId,
    string PokemonName,
    string Category,
    string Sustainability,
    string Season,
    int PrepMinutes,
    int Calories,
    int ProteinGrams,
    int FatGrams);

/// <summary>
/// The full seafood entry.
/// </summary>
public sealed record SeafoodDetailDto(
    int Id,
    string Slug,
    string Title,
    string RealName,
    int PokemonId,
    string PokemonName,
    string Category,
    string Sustainability,
    string Season,
    string Origin,
    string Preparation,
    string Flavor,
    string Pairing,
    int Calories,
    int ProteinGrams,
    int FatGrams,
    int PrepMinutes,
    string? Tips);

/// <summary>
/// The values available to filter the seafood guide.
/// </summary>
public sealed record SeafoodFacetsDto(
    IReadOnlyList<FacetDto> Categories,
    IReadOnlyList<FacetDto> Sustainabilities);
