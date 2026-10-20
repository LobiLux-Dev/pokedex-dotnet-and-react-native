using Microsoft.AspNetCore.Mvc;
using Pokedex.Application.Abstractions;
using Pokedex.Application.Dtos;

namespace Pokedex.Api.Controllers;

/// <summary>
/// Seafood guide endpoints.
/// </summary>
[ApiController]
[Route("api/v1/seafood")]
[Produces("application/json")]
public sealed class SeafoodController : ControllerBase
{
    private readonly ISeafoodService _seafoodService;

    public SeafoodController(ISeafoodService seafoodService)
    {
        _seafoodService = seafoodService;
    }

    /// <summary>
    /// Values available to filter the seafood guide.
    /// </summary>
    [HttpGet("facets")]
    [ProducesResponseType(typeof(SeafoodFacetsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SeafoodFacetsDto>> GetFacets(CancellationToken cancellationToken = default)
    {
        return Ok(await _seafoodService.GetFacetsAsync(cancellationToken));
    }

    /// <summary>
    /// Lists the seafood entries with optional search and filters.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Amount of entries per page (1-50).</param>
    /// <param name="search">Matches the Pokémon, the real seafood, the flavour, the origin or the pairing.</param>
    /// <param name="category">Exact category, e.g. <c>Crustáceo</c>.</param>
    /// <param name="sustainability">Exact sustainability rating.</param>
    /// <param name="maxPrepMinutes">Keeps only the entries that need this many minutes or less.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<SeafoodSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<SeafoodSummaryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = SeafoodQuery.DefaultPageSize,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] string? sustainability = null,
        [FromQuery] int? maxPrepMinutes = null,
        CancellationToken cancellationToken = default)
    {
        var query = new SeafoodQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Category = category,
            Sustainability = sustainability,
            MaxPrepMinutes = maxPrepMinutes
        };

        return Ok(await _seafoodService.GetPageAsync(query, cancellationToken));
    }

    /// <summary>
    /// Returns a seafood entry addressed by slug or by identifier.
    /// </summary>
    [HttpGet("{idOrSlug}")]
    [ProducesResponseType(typeof(SeafoodDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeafoodDetailDto>> GetByIdOrSlug(
        string idOrSlug,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _seafoodService.GetDetailAsync(idOrSlug, cancellationToken));
    }
}
