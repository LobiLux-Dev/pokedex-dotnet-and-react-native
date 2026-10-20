namespace Pokedex.Domain;

/// <summary>
/// Represents a Pokémon type (fire, water, electric, ...).
/// </summary>
public sealed record PokemonType(string Name)
{
    public override string ToString() => Name;
}
