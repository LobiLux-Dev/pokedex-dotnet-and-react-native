namespace Pokedex.Domain;

/// <summary>
/// A Pokémon type together with how many Pokémon belong to it.
/// </summary>
public sealed record PokemonTypeInfo(string Name, int PokemonCount);
