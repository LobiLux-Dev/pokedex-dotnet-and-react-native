using Microsoft.AspNetCore.Mvc;
using Pokedex.Application.Abstractions;
using Pokedex.Application.Dtos;

namespace Pokedex.Api.Controllers;

/// <summary>
/// Pokémon catalogue endpoints.
/// </summary>
[ApiController]
[Route("api/v1/pokemon")]
[Produces("application/json")]
public sealed class PokemonController : ControllerBase
{
    private readonly IPokemonService _pokemonService;

    public PokemonController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    /// <summary>
    /// Lists the Pokémon catalogue with optional search and type filters.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Amount of items per page (1-100).</param>
    /// <param name="search">Matches anywhere in the name or in the id.</param>
    /// <param name="type">Exact type filter, e.g. <c>electric</c>.</param>
    /// <param name="generation">National Pokédex generation (1-9).</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PokemonSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PokemonSummaryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PokemonQuery.DefaultPageSize,
        [FromQuery] string? search = null,
        [FromQuery] string? type = null,
        [FromQuery] int? generation = null,
        CancellationToken cancellationToken = default)
    {
        var query = new PokemonQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Type = type,
            Generation = generation
        };

        var result = await _pokemonService.GetPokemonPageAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns the full details of a Pokémon, addressed by name or by national Pokédex number.
    /// </summary>
    [HttpGet("{nameOrId}")]
    [ProducesResponseType(typeof(PokemonDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PokemonDetailDto>> GetByNameOrId(
        string nameOrId,
        CancellationToken cancellationToken = default)
    {
        var result = await _pokemonService.GetPokemonDetailAsync(nameOrId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns the evolution chain of a Pokémon.
    /// </summary>
    [HttpGet("{nameOrId}/evolution")]
    [ProducesResponseType(typeof(EvolutionChainDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EvolutionChainDto?>> GetEvolution(
        string nameOrId,
        CancellationToken cancellationToken = default)
    {
        var detail = await _pokemonService.GetPokemonDetailAsync(nameOrId, cancellationToken);
        return Ok(detail.Evolution);
    }
}
