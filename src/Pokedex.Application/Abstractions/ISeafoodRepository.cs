using Pokedex.Domain.Seafood;

namespace Pokedex.Application.Abstractions;

/// <summary>
/// Read access to the seafood guide.
/// </summary>
public interface ISeafoodRepository
{
    Task<IReadOnlyList<SeafoodEntry>> GetAllAsync(CancellationToken cancellationToken);

    Task<SeafoodEntry?> GetByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken);
}
