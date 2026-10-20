using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.PokeApi.Contracts;

/// <summary>
/// Raw <c>/pokemon-species/{name}</c> payload.
/// </summary>
internal sealed class PokeApiSpecies
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("genus")]
    public string? Genus { get; set; }

    [JsonPropertyName("habitat")]
    public PokeApiNamedResource? Habitat { get; set; }

    [JsonPropertyName("color")]
    public PokeApiNamedResource? Color { get; set; }

    [JsonPropertyName("evolution_chain")]
    public PokeApiNamedResource? EvolutionChain { get; set; }

    [JsonPropertyName("is_legendary")]
    public bool IsLegendary { get; set; }

    [JsonPropertyName("is_mythical")]
    public bool IsMythical { get; set; }

    [JsonPropertyName("flavor_text_entries")]
    public List<PokeApiFlavorTextEntry> FlavorTextEntries { get; set; } = [];

    [JsonPropertyName("genera")]
    public List<PokeApiGenusEntry> Genera { get; set; } = [];
}

internal sealed class PokeApiFlavorTextEntry
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public PokeApiNamedResource? Language { get; set; }
}

internal sealed class PokeApiGenusEntry
{
    [JsonPropertyName("genus")]
    public string Genus { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public PokeApiNamedResource? Language { get; set; }
}
