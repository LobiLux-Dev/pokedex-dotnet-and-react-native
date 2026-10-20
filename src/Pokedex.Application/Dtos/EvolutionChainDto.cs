namespace Pokedex.Application.Dtos;

/// <summary>
/// An evolution chain.
/// </summary>
public sealed record EvolutionChainDto(int Id, EvolutionNodeDto Root);

/// <summary>
/// A node of an evolution chain.
/// </summary>
public sealed record EvolutionNodeDto(
    string SpeciesName,
    IReadOnlyList<EvolutionDetailDto> Details,
    IReadOnlyList<EvolutionNodeDto> EvolvesTo);

/// <summary>
/// The condition that triggers an evolution.
/// </summary>
public sealed record EvolutionDetailDto(
    string? Trigger,
    int? MinLevel,
    string? Item,
    string? HeldItem,
    string? KnownMove,
    string? TimeOfDay,
    bool? TurnUpsideDown,
    int? Gender);
