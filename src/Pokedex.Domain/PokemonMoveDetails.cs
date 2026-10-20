namespace Pokedex.Domain;

/// <summary>
/// Extra information about a move (type, power, accuracy).
/// </summary>
public sealed record PokemonMoveDetails(
    string Name,
    string? Type,
    string? DamageClass,
    int? Power,
    int? Accuracy);
