namespace Pokedex.Application.Dtos;

/// <summary>
/// Full details of a Pokémon.
/// </summary>
public sealed record PokemonDetailDto(
    int Id,
    string Name,
    string DisplayName,
    IReadOnlyList<string> Types,
    double HeightInMeters,
    double WeightInKg,
    int? BaseExperience,
    int TotalStats,
    IReadOnlyList<PokemonAbilityDto> Abilities,
    IReadOnlyList<PokemonStatDto> Stats,
    PokemonSpritesDto Sprites,
    string? Genus,
    string? Description,
    string? Habitat,
    string? Color,
    bool IsLegendary,
    bool IsMythical,
    IReadOnlyList<PokemonMoveDto> Moves,
    EvolutionChainDto? Evolution);

/// <summary>
/// An ability of a Pokémon.
/// </summary>
public sealed record PokemonAbilityDto(string Name, bool IsHidden);

/// <summary>
/// A base stat of a Pokémon.
/// </summary>
public sealed record PokemonStatDto(string Name, int BaseValue, int Effort);

/// <summary>
/// Sprite URLs of a Pokémon.
/// </summary>
public sealed record PokemonSpritesDto(
    string? FrontDefault,
    string? FrontShiny,
    string? BackDefault,
    string? BackShiny,
    string? FrontFemale,
    string? FrontShinyFemale);

/// <summary>
/// A move learnable by a Pokémon.
/// </summary>
public sealed record PokemonMoveDto(
    string Name,
    string? Type,
    string? DamageClass,
    int? Power,
    int? Accuracy,
    int Level,
    string LearnMethod,
    string VersionGroup);
