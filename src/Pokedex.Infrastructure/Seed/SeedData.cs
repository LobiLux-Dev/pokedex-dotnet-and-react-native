using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pokedex.Domain.Recipes;
using Pokedex.Domain.Seafood;

namespace Pokedex.Infrastructure.Seed;

/// <summary>
/// Loads the editorial catalogues that ship with the API.
/// </summary>
public static class SeedData
{
    private const string RecipeFileName = "recipes.seed.json";
    private const string SeafoodFileName = "seafood.seed.json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<IReadOnlyList<Recipe>> LoadRecipesAsync(
        CancellationToken cancellationToken,
        ILogger logger)
    {
        var file = await ReadAsync<RecipeSeedFile>(RecipeFileName, cancellationToken);

        var recipes = file.Recipes
            .Select(recipe => new Recipe(
                recipe.Id,
                recipe.Slug,
                recipe.Title,
                recipe.Summary,
                recipe.Description,
                recipe.Category,
                recipe.Difficulty,
                recipe.PrepMinutes,
                recipe.CookMinutes,
                recipe.Servings,
                recipe.Tags,
                recipe.Allergens,
                recipe.Tips,
                recipe.Pokemon.Select(pokemon => new RecipePokemon(pokemon.Id, pokemon.Name)).ToList(),
                recipe.Ingredients
                    .Select(ingredient => new RecipeIngredient(
                        ingredient.Name,
                        ingredient.Quantity,
                        ingredient.Unit,
                        ingredient.Optional))
                    .ToList(),
                recipe.Steps
                    .Select(step => new RecipeStep(
                        step.Order,
                        step.Title,
                        step.Description,
                        step.DurationMinutes))
                    .ToList()))
            .OrderBy(recipe => recipe.Id)
            .ToList();

        logger.LogInformation("Recipe catalogue loaded with {Count} recipes.", recipes.Count);
        return recipes;
    }

    public static async Task<IReadOnlyList<SeafoodEntry>> LoadSeafoodAsync(
        CancellationToken cancellationToken,
        ILogger logger)
    {
        var file = await ReadAsync<SeafoodSeedFile>(SeafoodFileName, cancellationToken);

        var entries = file.Entries
            .Select(entry => new SeafoodEntry(
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
                entry.Tips))
            .OrderBy(entry => entry.Id)
            .ToList();

        logger.LogInformation("Seafood catalogue loaded with {Count} entries.", entries.Count);
        return entries;
    }

    private static async Task<T> ReadAsync<T>(string fileName, CancellationToken cancellationToken)
    {
        var assembly = typeof(SeedData).Assembly;
        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                $"The seed file '{fileName}' is missing from the assembly resources.");

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"The seed file '{fileName}' could not be opened.");

        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"The seed file '{fileName}' is empty or invalid.");
    }
}
