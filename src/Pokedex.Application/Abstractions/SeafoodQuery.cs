namespace Pokedex.Application.Abstractions;

/// <summary>
/// Query parameters of the seafood guide.
/// </summary>
public sealed record SeafoodQuery
{
    public const int DefaultPageSize = 12;
    public const int MaxPageSize = 50;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>Free text search over the name, the real seafood, the flavour and the origin.</summary>
    public string? Search { get; init; }

    public string? Category { get; init; }

    public string? Sustainability { get; init; }

    /// <summary>Keeps only the entries that need this many minutes or less to prepare.</summary>
    public int? MaxPrepMinutes { get; init; }

    public SeafoodQuery Normalize() => this with
    {
        Page = Math.Max(Page, 1),
        PageSize = Math.Clamp(PageSize, 1, MaxPageSize),
        Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim().ToLowerInvariant(),
        Category = string.IsNullOrWhiteSpace(Category) ? null : Category.Trim().ToLowerInvariant(),
        Sustainability = string.IsNullOrWhiteSpace(Sustainability) ? null : Sustainability.Trim().ToLowerInvariant(),
        MaxPrepMinutes = MaxPrepMinutes is <= 0 ? null : MaxPrepMinutes
    };
}
