namespace Pokedex.Application.Dtos;

/// <summary>
/// A recipe as shown in the catalogue list.
/// </summary>
public sealed record RecipeSummaryDto(
    int Id,
    string Slug,
    string Title,
    string Summary,
    string Category,
    string Difficulty,
    int PrepMinutes,
    int CookMinutes,
    int TotalMinutes,
    int Servings,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> Allergens,
    IReadOnlyList<RecipePokemonDto> Pokemon,
    int IngredientCount,
    int StepCount);

/// <summary>
/// The full recipe.
/// </summary>
public sealed record RecipeDetailDto(
    int Id,
    string Slug,
    string Title,
    string Summary,
    string Description,
    string Category,
    string Difficulty,
    int PrepMinutes,
    int CookMinutes,
    int TotalMinutes,
    int Servings,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> Allergens,
    string? Tips,
    IReadOnlyList<RecipePokemonDto> Pokemon,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    IReadOnlyList<RecipeStepDto> Steps);

/// <summary>
/// A Pokémon referenced by a recipe.
/// </summary>
public sealed record RecipePokemonDto(int Id, string Name, string DisplayName);

/// <summary>
/// An ingredient of a recipe.
/// </summary>
public sealed record RecipeIngredientDto(string Name, string Quantity, string Unit, bool Optional);

/// <summary>
/// A step of a recipe.
/// </summary>
public sealed record RecipeStepDto(int Order, string Title, string Description, int? DurationMinutes);

/// <summary>
/// The values available to filter the recipe catalogue.
/// </summary>
public sealed record RecipeFacetsDto(
    IReadOnlyList<FacetDto> Categories,
    IReadOnlyList<FacetDto> Difficulties,
    IReadOnlyList<FacetDto> Tags);

/// <summary>
/// A filterable value and how many recipes use it.
/// </summary>
public sealed record FacetDto(string Name, int Count);
