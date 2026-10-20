namespace Pokedex.Domain;

/// <summary>
/// An ability a Pokémon can have, optionally hidden (not shown in the summary UI).
/// </summary>
public sealed record PokemonAbility(string Name, bool IsHidden);
