using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Pokedex.Application.Abstractions;
using Pokedex.Domain.Recipes;
using Pokedex.Infrastructure.Seed;

namespace Pokedex.Infrastructure.Catalog;

/// <summary>
/// Reads the recipe catalogue from the seed files embedded in the assembly.
/// </summary>
public sealed class RecipeRepository : IRecipeRepository
{
    private const string CacheKey = "catalog:recipes";

    private readonly HybridCache _cache;
    private readonly ILogger<RecipeRepository> _logger;

    public RecipeRepository(HybridCache cache, ILogger<RecipeRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromHours(12)
        };

        var recipes = await _cache.GetOrCreateAsync<IReadOnlyList<Recipe>>(
            CacheKey,
            async ct => await SeedData.LoadRecipesAsync(ct, _logger),
            entryOptions);

        return recipes ?? [];
    }

    public async Task<Recipe?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken)
    {
        var recipes = await GetAllAsync(cancellationToken);
        var trimmed = idOrSlug.Trim();

        return recipes.FirstOrDefault(recipe =>
                string.Equals(recipe.Slug, trimmed, StringComparison.OrdinalIgnoreCase))
            ?? (int.TryParse(trimmed, out var id)
                ? recipes.FirstOrDefault(recipe => recipe.Id == id)
                : null);
    }
}
