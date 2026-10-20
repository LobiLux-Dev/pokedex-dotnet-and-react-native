namespace Pokedex.Application.Common;

/// <summary>
/// Thrown when a Pokémon does not exist in PokeAPI.
/// </summary>
public sealed class PokemonNotFoundException : Exception
{
    public PokemonNotFoundException(string nameOrId)
        : base($"Pokémon '{nameOrId}' was not found.")
    {
        NameOrId = nameOrId;
    }

    public string NameOrId { get; }
}
