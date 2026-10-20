using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.PokeApi.Contracts;

/// <summary>
/// A paginated list of named resources.
/// </summary>
internal sealed class PokeApiNamedResourceList
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("results")]
    public List<PokeApiNamedResource> Results { get; set; } = [];
}

/// <summary>
/// A named resource pointing to another PokeAPI endpoint.
/// </summary>
internal sealed class PokeApiNamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
