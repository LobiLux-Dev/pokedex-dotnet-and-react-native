using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.PokeApi.Contracts;

/// <summary>
/// Raw <c>/pokemon/{name}</c> payload.
/// </summary>
internal sealed class PokeApiPokemon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("base_experience")]
    public int? BaseExperience { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("types")]
    public List<PokeApiTypeSlot> Types { get; set; } = [];

    [JsonPropertyName("abilities")]
    public List<PokeApiAbilitySlot> Abilities { get; set; } = [];

    [JsonPropertyName("stats")]
    public List<PokeApiStatEntry> Stats { get; set; } = [];

    [JsonPropertyName("moves")]
    public List<PokeApiMoveEntry> Moves { get; set; } = [];

    [JsonPropertyName("sprites")]
    public PokeApiSprites? Sprites { get; set; }

    [JsonPropertyName("species")]
    public PokeApiNamedResource? Species { get; set; }
}

internal sealed class PokeApiTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonPropertyName("type")]
    public PokeApiNamedResource? Type { get; set; }
}

internal sealed class PokeApiAbilitySlot
{
    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }

    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonPropertyName("ability")]
    public PokeApiNamedResource? Ability { get; set; }
}

internal sealed class PokeApiStatEntry
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; set; }

    [JsonPropertyName("effort")]
    public int Effort { get; set; }

    [JsonPropertyName("stat")]
    public PokeApiNamedResource? Stat { get; set; }
}

internal sealed class PokeApiMoveEntry
{
    [JsonPropertyName("move")]
    public PokeApiNamedResource? Move { get; set; }

    [JsonPropertyName("version_group_details")]
    public List<PokeApiVersionGroupDetail> VersionGroupDetails { get; set; } = [];
}

internal sealed class PokeApiVersionGroupDetail
{
    [JsonPropertyName("level_learned_at")]
    public int LevelLearnedAt { get; set; }

    [JsonPropertyName("move_learn_method")]
    public PokeApiNamedResource? MoveLearnMethod { get; set; }

    [JsonPropertyName("version_group")]
    public PokeApiNamedResource? VersionGroup { get; set; }
}

internal sealed class PokeApiSprites
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; set; }

    [JsonPropertyName("front_female")]
    public string? FrontFemale { get; set; }

    [JsonPropertyName("front_shiny_female")]
    public string? FrontShinyFemale { get; set; }

    [JsonPropertyName("back_default")]
    public string? BackDefault { get; set; }

    [JsonPropertyName("back_shiny")]
    public string? BackShiny { get; set; }

    [JsonPropertyName("back_female")]
    public string? BackFemale { get; set; }

    [JsonPropertyName("back_shiny_female")]
    public string? BackShinyFemale { get; set; }
}
