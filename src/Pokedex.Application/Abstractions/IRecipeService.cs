using Pokedex.Application.Dtos;

namespace Pokedex.Application.Abstractions;

/// <summary>
/// Recipe use cases.
/// </summary>
public interface IRecipeService
{
    Task<PagedResponse<RecipeSummaryDto>> GetPageAsync(RecipeQuery query, CancellationToken cancellationToken);

    Task<RecipeDetailDto> GetDetailAsync(string idOrSlug, CancellationToken cancellationToken);

    Task<RecipeFacetsDto> GetFacetsAsync(CancellationToken cancellationToken);
}
