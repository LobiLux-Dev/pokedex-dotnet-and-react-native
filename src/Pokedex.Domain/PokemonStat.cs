namespace Pokedex.Domain;

/// <summary>
/// A base stat (hp, attack, ...) of a Pokémon.
/// </summary>
public sealed record PokemonStat(string Name, int BaseValue, int Effort);
