using Pokedex.Application.Abstractions;
using Pokedex.Application.Common;
using Pokedex.Application.Dtos;
using Pokedex.Application.Mapping;
using Pokedex.Domain.Seafood;

namespace Pokedex.Application.Services;

/// <summary>
/// Implements the seafood guide use cases on top of <see cref="ISeafoodRepository"/>.
/// </summary>
public sealed class SeafoodService : ISeafoodService
{
    private readonly ISeafoodRepository _repository;

    public SeafoodService(ISeafoodRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<SeafoodSummaryDto>> GetPageAsync(
        SeafoodQuery query,
        CancellationToken cancellationToken)
    {
        var normalized = query.Normalize();
        var catalogue = await _repository.GetAllAsync(cancellationToken);

        IEnumerable<SeafoodEntry> filtered = catalogue;

        if (normalized.Search is not null)
        {
            filtered = filtered.Where(entry => Matches(entry, normalized.Search));
        }

        if (normalized.Category is not null)
        {
            filtered = filtered.Where(entry =>
                string.Equals(entry.Category, normalized.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (normalized.Sustainability is not null)
        {
            filtered = filtered.Where(entry =>
                string.Equals(entry.Sustainability, normalized.Sustainability, StringComparison.OrdinalIgnoreCase));
        }

        if (normalized.MaxPrepMinutes is int maxMinutes)
        {
            filtered = filtered.Where(entry => entry.PrepMinutes <= maxMinutes);
        }

        var ordered = filtered
            .OrderBy(entry => entry.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var items = ordered
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(CatalogMapper.ToSummaryDto)
            .ToList();

        return PagedResponse<SeafoodSummaryDto>.Create(
            items,
            normalized.Page,
            normalized.PageSize,
            ordered.Count);
    }

    public async Task<SeafoodDetailDto> GetDetailAsync(string idOrSlug, CancellationToken cancellationToken)
    {
        var entry = await _repository.GetByIdOrSlugAsync(idOrSlug, cancellationToken);
        if (entry is null)
        {
            throw new ResourceNotFoundException("Entrada de marisco", idOrSlug);
        }

        return entry.ToDetailDto();
    }

    public async Task<SeafoodFacetsDto> GetFacetsAsync(CancellationToken cancellationToken)
    {
        var catalogue = await _repository.GetAllAsync(cancellationToken);

        var categories = catalogue
            .GroupBy(entry => entry.Category, StringComparer.OrdinalIgnoreCase)
            .Select(group => new FacetDto(group.Key, group.Count()))
            .OrderBy(facet => facet.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var sustainabilities = catalogue
            .GroupBy(entry => entry.Sustainability, StringComparer.OrdinalIgnoreCase)
            .Select(group => new FacetDto(group.Key, group.Count()))
            .OrderBy(facet => Array.IndexOf(
                ["Excelente", "Buena", "Moderada", "Poco recomendada"],
                facet.Name))
            .ToList();

        return new SeafoodFacetsDto(categories, sustainabilities);
    }

    /// <summary>
    /// The search matches the Pokémon, the real seafood, the flavour, the origin and the pairing.
    /// </summary>
    private static bool Matches(SeafoodEntry entry, string search) =>
        entry.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
        || entry.RealName.Contains(search, StringComparison.OrdinalIgnoreCase)
        || entry.PokemonName.Contains(search, StringComparison.OrdinalIgnoreCase)
        || entry.Flavor.Contains(search, StringComparison.OrdinalIgnoreCase)
        || entry.Origin.Contains(search, StringComparison.OrdinalIgnoreCase)
        || entry.Pairing.Contains(search, StringComparison.OrdinalIgnoreCase);
}
