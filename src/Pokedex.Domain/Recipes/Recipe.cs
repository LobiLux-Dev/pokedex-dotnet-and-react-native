namespace Pokedex.Domain.Recipes;

/// <summary>
/// A recipe inspired by a Pokémon.
/// </summary>
public sealed record Recipe(
    int Id,
    string Slug,
    string Title,
    string Summary,
    string Description,
    string Category,
    string Difficulty,
    int PrepMinutes,
    int CookMinutes,
    int Servings,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> Allergens,
    string? Tips,
    IReadOnlyList<RecipePokemon> Pokemon,
    IReadOnlyList<RecipeIngredient> Ingredients,
    IReadOnlyList<RecipeStep> Steps)
{
    /// <summary>Total time needed from start to finish, in minutes.</summary>
    public int TotalMinutes => PrepMinutes + CookMinutes;
}

/// <summary>
/// A Pokémon used in a recipe.
/// </summary>
public sealed record RecipePokemon(int Id, string Name);

/// <summary>
/// An ingredient of a recipe.
/// </summary>
public sealed record RecipeIngredient(string Name, string Quantity, string Unit, bool Optional);

/// <summary>
/// A step of a recipe.
/// </summary>
public sealed record RecipeStep(int Order, string Title, string Description, int? DurationMinutes);
