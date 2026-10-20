using Pokedex.Application.Abstractions;
using Pokedex.Application.Common;
using Pokedex.Application.Dtos;
using Pokedex.Application.Mapping;
using Pokedex.Domain.Recipes;

namespace Pokedex.Application.Services;

/// <summary>
/// Implements the recipe use cases on top of <see cref="IRecipeRepository"/>.
/// </summary>
public sealed class RecipeService : IRecipeService
{
    private static readonly string[] DifficultyOrder = ["Fácil", "Media", "Difícil"];

    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<RecipeSummaryDto>> GetPageAsync(
        RecipeQuery query,
        CancellationToken cancellationToken)
    {
        var normalized = query.Normalize();
        var catalogue = await _repository.GetAllAsync(cancellationToken);

        IEnumerable<Recipe> filtered = catalogue;

        if (normalized.Search is not null)
        {
            filtered = filtered.Where(recipe => Matches(recipe, normalized.Search));
        }

        if (normalized.Category is not null)
        {
            filtered = filtered.Where(recipe =>
                string.Equals(recipe.Category, normalized.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (normalized.Difficulty is not null)
        {
            filtered = filtered.Where(recipe =>
                string.Equals(recipe.Difficulty, normalized.Difficulty, StringComparison.OrdinalIgnoreCase));
        }

        if (normalized.Tag is not null)
        {
            filtered = filtered.Where(recipe =>
                recipe.Tags.Contains(normalized.Tag, StringComparer.OrdinalIgnoreCase));
        }

        if (normalized.MaxMinutes is int maxMinutes)
        {
            filtered = filtered.Where(recipe => recipe.TotalMinutes <= maxMinutes);
        }

        var ordered = filtered
            .OrderBy(recipe => recipe.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var items = ordered
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(CatalogMapper.ToSummaryDto)
            .ToList();

        return PagedResponse<RecipeSummaryDto>.Create(
            items,
            normalized.Page,
            normalized.PageSize,
            ordered.Count);
    }

    public async Task<RecipeDetailDto> GetDetailAsync(string idOrSlug, CancellationToken cancellationToken)
    {
        var recipe = await _repository.GetByIdOrSlugAsync(idOrSlug, cancellationToken);
        if (recipe is null)
        {
            throw new ResourceNotFoundException("Receta", idOrSlug);
        }

        return recipe.ToDetailDto();
    }

    public async Task<RecipeFacetsDto> GetFacetsAsync(CancellationToken cancellationToken)
    {
        var catalogue = await _repository.GetAllAsync(cancellationToken);

        var categories = catalogue
            .GroupBy(recipe => recipe.Category, StringComparer.OrdinalIgnoreCase)
            .Select(group => new FacetDto(group.Key, group.Count()))
            .OrderBy(facet => facet.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var difficulties = catalogue
            .GroupBy(recipe => recipe.Difficulty, StringComparer.OrdinalIgnoreCase)
            .Select(group => new FacetDto(group.Key, group.Count()))
            .OrderBy(facet => Array.IndexOf(DifficultyOrder, facet.Name))
            .ToList();

        var tags = catalogue
            .SelectMany(recipe => recipe.Tags)
            .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
            .Select(group => new FacetDto(group.Key, group.Count()))
            .OrderBy(facet => facet.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return new RecipeFacetsDto(categories, difficulties, tags);
    }

    /// <summary>
    /// The search matches the title, the summary, the tags, the ingredients and the Pokémon used.
    /// </summary>
    private static bool Matches(Recipe recipe, string search) =>
        recipe.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
        || recipe.Summary.Contains(search, StringComparison.OrdinalIgnoreCase)
        || recipe.Tags.Any(tag => tag.Contains(search, StringComparison.OrdinalIgnoreCase))
        || recipe.Ingredients.Any(ingredient => ingredient.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
        || recipe.Pokemon.Any(pokemon => pokemon.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
}
