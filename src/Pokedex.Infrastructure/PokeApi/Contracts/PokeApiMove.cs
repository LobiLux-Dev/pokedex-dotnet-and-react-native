using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.PokeApi.Contracts;

/// <summary>
/// Raw <c>/move/{name}</c> payload.
/// </summary>
internal sealed class PokeApiMove
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("accuracy")]
    public int? Accuracy { get; set; }

    [JsonPropertyName("power")]
    public int? Power { get; set; }

    [JsonPropertyName("type")]
    public PokeApiNamedResource? Type { get; set; }

    [JsonPropertyName("damage_class")]
    public PokeApiNamedResource? DamageClass { get; set; }
}
