using Microsoft.AspNetCore.Mvc;
using Pokedex.Application.Abstractions;
using Pokedex.Application.Dtos;

namespace Pokedex.Api.Controllers;

/// <summary>
/// Pokémon type endpoints, used to build the type filter.
/// </summary>
[ApiController]
[Route("api/v1/types")]
[Produces("application/json")]
public sealed class TypesController : ControllerBase
{
    private readonly IPokemonService _pokemonService;

    public TypesController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    /// <summary>
    /// Returns every Pokémon type together with the amount of Pokémon that belong to it.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PokemonTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PokemonTypeDto>>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var types = await _pokemonService.GetTypesAsync(cancellationToken);
        return Ok(types);
    }
}
