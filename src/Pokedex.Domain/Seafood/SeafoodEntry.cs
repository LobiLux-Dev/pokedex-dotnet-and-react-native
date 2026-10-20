namespace Pokedex.Domain.Seafood;

/// <summary>
/// An entry of the seafood guide: a Pokémon and the real seafood it represents.
/// </summary>
public sealed record SeafoodEntry(
    int Id,
    string Slug,
    string Title,
    string RealName,
    int PokemonId,
    string PokemonName,
    string Category,
    string Sustainability,
    string Season,
    string Origin,
    string Preparation,
    string Flavor,
    string Pairing,
    int Calories,
    int ProteinGrams,
    int FatGrams,
    int PrepMinutes,
    string? Tips);
