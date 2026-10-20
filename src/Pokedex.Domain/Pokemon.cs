namespace Pokedex.Domain;

/// <summary>
/// Full Pokémon details.
/// </summary>
public sealed record Pokemon(
    int Id,
    string Name,
    int Height,
    int Weight,
    int? BaseExperience,
    IReadOnlyList<PokemonType> Types,
    IReadOnlyList<PokemonAbility> Abilities,
    IReadOnlyList<PokemonStat> Stats,
    PokemonSprites Sprites,
    string SpeciesName,
    IReadOnlyList<PokemonMove> Moves)
{
    /// <summary>Sum of every base stat (the classic "base stat total").</summary>
    public int TotalStats => Stats.Sum(stat => stat.BaseValue);

    /// <summary>Weight expressed in kilograms.</summary>
    public double WeightInKg => Weight / 10d;

    /// <summary>Height expressed in metres.</summary>
    public double HeightInMeters => Height / 10d;
}
