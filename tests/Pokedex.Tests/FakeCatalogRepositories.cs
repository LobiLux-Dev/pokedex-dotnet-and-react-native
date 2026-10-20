using Pokedex.Application.Abstractions;
using Pokedex.Domain.Recipes;
using Pokedex.Domain.Seafood;

namespace Pokedex.Tests;

/// <summary>
/// In-memory recipe repository used by the tests.
/// </summary>
public sealed class FakeRecipeRepository : IRecipeRepository
{
    public List<Recipe> Recipes { get; } = [];

    public Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Recipe>>(Recipes);

    public Task<Recipe?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken) =>
        Task.FromResult(Recipes.FirstOrDefault(recipe =>
                string.Equals(recipe.Slug, idOrSlug, StringComparison.OrdinalIgnoreCase))
            ?? Recipes.FirstOrDefault(recipe => recipe.Id.ToString() == idOrSlug));
}

/// <summary>
/// In-memory seafood repository used by the tests.
/// </summary>
public sealed class FakeSeafoodRepository : ISeafoodRepository
{
    public List<SeafoodEntry> Entries { get; } = [];

    public Task<IReadOnlyList<SeafoodEntry>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<SeafoodEntry>>(Entries);

    public Task<SeafoodEntry?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken) =>
        Task.FromResult(Entries.FirstOrDefault(entry =>
                string.Equals(entry.Slug, idOrSlug, StringComparison.OrdinalIgnoreCase))
            ?? Entries.FirstOrDefault(entry => entry.Id.ToString() == idOrSlug));
}
