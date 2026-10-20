namespace Pokedex.Application.Abstractions;

using Pokedex.Application.Dtos;

/// <summary>
/// Application use cases exposed to the API layer.
/// </summary>
public interface IPokemonService
{
    Task<PagedResponse<PokemonSummaryDto>> GetPokemonPageAsync(PokemonQuery query, CancellationToken cancellationToken);

    Task<PokemonDetailDto> GetPokemonDetailAsync(string nameOrId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PokemonTypeDto>> GetTypesAsync(CancellationToken cancellationToken);
}
