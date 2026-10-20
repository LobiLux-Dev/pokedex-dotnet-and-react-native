using Pokedex.Application.Dtos;
using Pokedex.Domain.Recipes;
using Pokedex.Domain.Seafood;

namespace Pokedex.Application.Mapping;

/// <summary>
/// Maps the recipe and seafood catalogues to the DTOs exposed by the API.
/// </summary>
public static class CatalogMapper
{
    public static RecipeSummaryDto ToSummaryDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Slug,
        recipe.Title,
        recipe.Summary,
        recipe.Category,
        recipe.Difficulty,
        recipe.PrepMinutes,
        recipe.CookMinutes,
        recipe.TotalMinutes,
        recipe.Servings,
        recipe.Tags,
        recipe.Allergens,
        recipe.Pokemon.Select(pokemon => new RecipePokemonDto(
            pokemon.Id,
            pokemon.Name,
            PokemonMapper.ToDisplayName(pokemon.Name))).ToList(),
        recipe.Ingredients.Count,
        recipe.Steps.Count);

    public static RecipeDetailDto ToDetailDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Slug,
        recipe.Title,
        recipe.Summary,
        recipe.Description,
        recipe.Category,
        recipe.Difficulty,
        recipe.PrepMinutes,
        recipe.CookMinutes,
        recipe.TotalMinutes,
        recipe.Servings,
        recipe.Tags,
        recipe.Allergens,
        recipe.Tips,
        recipe.Pokemon.Select(pokemon => new RecipePokemonDto(
            pokemon.Id,
            pokemon.Name,
            PokemonMapper.ToDisplayName(pokemon.Name))).ToList(),
        recipe.Ingredients.Select(ingredient => new RecipeIngredientDto(
            ingredient.Name,
            ingredient.Quantity,
            ingredient.Unit,
            ingredient.Optional)).ToList(),
        recipe.Steps.OrderBy(step => step.Order).Select(step => new RecipeStepDto(
            step.Order,
            step.Title,
            step.Description,
            step.DurationMinutes)).ToList());

    public static SeafoodSummaryDto ToSummaryDto(this SeafoodEntry entry) => new(
        entry.Id,
        entry.Slug,
        entry.Title,
        entry.RealName,
        entry.PokemonId,
        entry.PokemonName,
        entry.Category,
        entry.Sustainability,
        entry.Season,
        entry.PrepMinutes,
        entry.Calories,
        entry.ProteinGrams,
        entry.FatGrams);

    public static SeafoodDetailDto ToDetailDto(this SeafoodEntry entry) => new(
        entry.Id,
        entry.Slug,
        entry.Title,
        entry.RealName,
        entry.PokemonId,
        entry.PokemonName,
        entry.Category,
        entry.Sustainability,
        entry.Season,
        entry.Origin,
        entry.Preparation,
        entry.Flavor,
        entry.Pairing,
        entry.Calories,
        entry.ProteinGrams,
        entry.FatGrams,
        entry.PrepMinutes,
        entry.Tips);
}
