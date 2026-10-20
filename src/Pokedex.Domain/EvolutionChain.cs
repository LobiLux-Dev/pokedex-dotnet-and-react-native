namespace Pokedex.Domain;

/// <summary>
/// A node of an evolution chain.
/// </summary>
public sealed record EvolutionNode(
    string SpeciesName,
    IReadOnlyList<EvolutionDetail> Details,
    IReadOnlyList<EvolutionNode> EvolvesTo);

/// <summary>
/// The condition that triggers an evolution.
/// </summary>
public sealed record EvolutionDetail(
    string? Trigger,
    int? MinLevel,
    string? Item,
    string? HeldItem,
    string? KnownMove,
    string? TimeOfDay,
    bool? TurnUpsideDown,
    int? Gender);

/// <summary>
/// A full evolution chain.
/// </summary>
public sealed record EvolutionChain(int Id, EvolutionNode Root);
