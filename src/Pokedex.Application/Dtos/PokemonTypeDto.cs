namespace Pokedex.Application.Dtos;

/// <summary>
/// A Pokémon type and how many Pokémon belong to it.
/// </summary>
public sealed record PokemonTypeDto(string Name, int PokemonCount);
