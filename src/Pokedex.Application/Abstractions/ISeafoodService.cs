using Pokedex.Application.Dtos;

namespace Pokedex.Application.Abstractions;

/// <summary>
/// Seafood guide use cases.
/// </summary>
public interface ISeafoodService
{
    Task<PagedResponse<SeafoodSummaryDto>> GetPageAsync(SeafoodQuery query, CancellationToken cancellationToken);

    Task<SeafoodDetailDto> GetDetailAsync(string idOrSlug, CancellationToken cancellationToken);

    Task<SeafoodFacetsDto> GetFacetsAsync(CancellationToken cancellationToken);
}
