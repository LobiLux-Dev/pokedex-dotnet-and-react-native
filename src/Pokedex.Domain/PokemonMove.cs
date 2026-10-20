namespace Pokedex.Domain;

/// <summary>
/// A move learnable by a Pokémon.
/// </summary>
/// <param name="Name">Move name, e.g. <c>thunder-shock</c>.</param>
/// <param name="Type">Move type, e.g. <c>electric</c>. Null when the upstream detail could not be resolved.</param>
/// <param name="DamageClass">Move damage class, e.g. <c>special</c>. Null when unresolved.</param>
/// <param name="Power">Move power, null when unknown.</param>
/// <param name="Accuracy">Move accuracy in percent, null when unknown.</param>
/// <param name="Level">Level at which the move is learned.</param>
/// <param name="LearnMethod">How the move is learned, e.g. <c>level-up</c>.</param>
/// <param name="VersionGroup">Version group where the move is learned, e.g. <c>red-blue</c>.</param>
public sealed record PokemonMove(
    string Name,
    string? Type,
    string? DamageClass,
    int? Power,
    int? Accuracy,
    int Level,
    string LearnMethod,
    string VersionGroup);
