using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.PokeApi.Contracts;

/// <summary>
/// Raw <c>/evolution-chain/{id}</c> payload.
/// </summary>
internal sealed class PokeApiEvolutionChain
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("chain")]
    public PokeApiEvolutionNode? Chain { get; set; }
}

internal sealed class PokeApiEvolutionNode
{
    [JsonPropertyName("species")]
    public PokeApiNamedResource? Species { get; set; }

    [JsonPropertyName("evolution_details")]
    public List<PokeApiEvolutionDetail> EvolutionDetails { get; set; } = [];

    [JsonPropertyName("evolves_to")]
    public List<PokeApiEvolutionNode> EvolvesTo { get; set; } = [];
}

internal sealed class PokeApiEvolutionDetail
{
    [JsonPropertyName("min_level")]
    public int? MinLevel { get; set; }

    [JsonPropertyName("trigger")]
    public PokeApiNamedResource? Trigger { get; set; }

    [JsonPropertyName("item")]
    public PokeApiNamedResource? Item { get; set; }

    [JsonPropertyName("held_item")]
    public PokeApiNamedResource? HeldItem { get; set; }

    [JsonPropertyName("known_move")]
    public PokeApiNamedResource? KnownMove { get; set; }

    [JsonPropertyName("time_of_day")]
    public string? TimeOfDay { get; set; }

    [JsonPropertyName("turn_upside_down")]
    public bool? TurnUpsideDown { get; set; }

    [JsonPropertyName("gender")]
    public int? Gender { get; set; }
}
