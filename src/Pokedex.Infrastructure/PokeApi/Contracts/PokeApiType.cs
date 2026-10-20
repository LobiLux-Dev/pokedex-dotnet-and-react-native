using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.PokeApi.Contracts;

/// <summary>
/// Raw <c>/type/{name}</c> payload.
/// </summary>
internal sealed class PokeApiTypeDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The members of the type. PokeAPI nests the resource: <c>{"slot":1,"pokemon":{...}}</c>.
    /// </summary>
    [JsonPropertyName("pokemon")]
    public List<PokeApiTypePokemonEntry> Pokemon { get; set; } = [];
}

/// <summary>
/// An entry of the <c>pokemon</c> array of a type.
/// </summary>
internal sealed class PokeApiTypePokemonEntry
{
    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonPropertyName("pokemon")]
    public PokeApiNamedResource? Pokemon { get; set; }
}
