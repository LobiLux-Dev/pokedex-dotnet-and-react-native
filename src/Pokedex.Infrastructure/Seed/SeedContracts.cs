using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.Seed;

/// <summary>
/// Root of the recipe seed file.
/// </summary>
internal sealed class RecipeSeedFile
{
    [JsonPropertyName("recipes")]
    public List<RecipeSeed> Recipes { get; set; } = [];
}

internal sealed class RecipeSeed
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("prepMinutes")]
    public int PrepMinutes { get; set; }

    [JsonPropertyName("cookMinutes")]
    public int CookMinutes { get; set; }

    [JsonPropertyName("servings")]
    public int Servings { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("allergens")]
    public List<string> Allergens { get; set; } = [];

    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    [JsonPropertyName("pokemon")]
    public List<RecipePokemonSeed> Pokemon { get; set; } = [];

    [JsonPropertyName("ingredients")]
    public List<RecipeIngredientSeed> Ingredients { get; set; } = [];

    [JsonPropertyName("steps")]
    public List<RecipeStepSeed> Steps { get; set; } = [];
}

internal sealed class RecipePokemonSeed
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

internal sealed class RecipeIngredientSeed
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public string Quantity { get; set; } = string.Empty;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("optional")]
    public bool Optional { get; set; }
}

internal sealed class RecipeStepSeed
{
    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("durationMinutes")]
    public int? DurationMinutes { get; set; }
}

/// <summary>
/// Root of the seafood seed file.
/// </summary>
internal sealed class SeafoodSeedFile
{
    [JsonPropertyName("entries")]
    public List<SeafoodSeed> Entries { get; set; } = [];
}

internal sealed class SeafoodSeed
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("realName")]
    public string RealName { get; set; } = string.Empty;

    [JsonPropertyName("pokemonId")]
    public int PokemonId { get; set; }

    [JsonPropertyName("pokemonName")]
    public string PokemonName { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("sustainability")]
    public string Sustainability { get; set; } = string.Empty;

    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;

    [JsonPropertyName("origin")]
    public string Origin { get; set; } = string.Empty;

    [JsonPropertyName("preparation")]
    public string Preparation { get; set; } = string.Empty;

    [JsonPropertyName("flavor")]
    public string Flavor { get; set; } = string.Empty;

    [JsonPropertyName("pairing")]
    public string Pairing { get; set; } = string.Empty;

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("proteinGrams")]
    public int ProteinGrams { get; set; }

    [JsonPropertyName("fatGrams")]
    public int FatGrams { get; set; }

    [JsonPropertyName("prepMinutes")]
    public int PrepMinutes { get; set; }

    [JsonPropertyName("tips")]
    public string? Tips { get; set; }
}
