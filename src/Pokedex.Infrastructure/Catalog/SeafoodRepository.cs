using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Pokedex.Application.Abstractions;
using Pokedex.Domain.Seafood;
using Pokedex.Infrastructure.Seed;

namespace Pokedex.Infrastructure.Catalog;

/// <summary>
/// Reads the seafood guide from the seed files embedded in the assembly.
/// </summary>
public sealed class SeafoodRepository : ISeafoodRepository
{
    private const string CacheKey = "catalog:seafood";

    private readonly HybridCache _cache;
    private readonly ILogger<SeafoodRepository> _logger;

    public SeafoodRepository(HybridCache cache, ILogger<SeafoodRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SeafoodEntry>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromHours(12)
        };

        var entries = await _cache.GetOrCreateAsync<IReadOnlyList<SeafoodEntry>>(
            CacheKey,
            async ct => await SeedData.LoadSeafoodAsync(ct, _logger),
            entryOptions);

        return entries ?? [];
    }

    public async Task<SeafoodEntry?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken)
    {
        var entries = await GetAllAsync(cancellationToken);
        var trimmed = idOrSlug.Trim();

        return entries.FirstOrDefault(entry =>
                string.Equals(entry.Slug, trimmed, StringComparison.OrdinalIgnoreCase))
            ?? (int.TryParse(trimmed, out var id)
                ? entries.FirstOrDefault(entry => entry.Id == id)
                : null);
    }
}
