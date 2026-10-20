using Microsoft.AspNetCore.Mvc;
using Pokedex.Application.Abstractions;
using Pokedex.Application.Dtos;

namespace Pokedex.Api.Controllers;

/// <summary>
/// Recipe book endpoints.
/// </summary>
[ApiController]
[Route("api/v1/recipes")]
[Produces("application/json")]
public sealed class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Values available to filter the recipe book.
    /// </summary>
    [HttpGet("facets")]
    [ProducesResponseType(typeof(RecipeFacetsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RecipeFacetsDto>> GetFacets(CancellationToken cancellationToken = default)
    {
        return Ok(await _recipeService.GetFacetsAsync(cancellationToken));
    }

    /// <summary>
    /// Lists the recipes with optional search and filters.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Amount of recipes per page (1-50).</param>
    /// <param name="search">Matches the title, the summary, the tags, the ingredients or the Pokémon.</param>
    /// <param name="category">Exact category, e.g. <c>Postre</c>.</param>
    /// <param name="difficulty">Exact difficulty, e.g. <c>Fácil</c>.</param>
    /// <param name="maxMinutes">Keeps only the recipes that take this many minutes or less.</param>
    /// <param name="tag">Exact tag.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RecipeSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<RecipeSummaryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = RecipeQuery.DefaultPageSize,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] string? difficulty = null,
        [FromQuery] int? maxMinutes = null,
        [FromQuery] string? tag = null,
        CancellationToken cancellationToken = default)
    {
        var query = new RecipeQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Category = category,
            Difficulty = difficulty,
            MaxMinutes = maxMinutes,
            Tag = tag
        };

        return Ok(await _recipeService.GetPageAsync(query, cancellationToken));
    }

    /// <summary>
    /// Returns a recipe addressed by slug or by identifier.
    /// </summary>
    [HttpGet("{idOrSlug}")]
    [ProducesResponseType(typeof(RecipeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailDto>> GetByIdOrSlug(
        string idOrSlug,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _recipeService.GetDetailAsync(idOrSlug, cancellationToken));
    }
}
