using Pokedex.Domain.Recipes;

namespace Pokedex.Application.Abstractions;

/// <summary>
/// Read access to the recipe catalogue.
/// </summary>
public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken);

    Task<Recipe?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken);
}
