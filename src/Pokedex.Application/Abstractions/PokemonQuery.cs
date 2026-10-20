namespace Pokedex.Application.Abstractions;

/// <summary>
/// Query parameters for the Pokémon catalogue endpoint.
/// </summary>
public sealed record PokemonQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>1-based page number.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Amount of items per page.</summary>
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>Free text search, matched against the name and the id.</summary>
    public string? Search { get; init; }

    /// <summary>Exact type filter, e.g. <c>electric</c>.</summary>
    public string? Type { get; init; }

    /// <summary>National Pokédex generation (1 to 9).</summary>
    public int? Generation { get; init; }

    /// <summary>Clamps out of range values and trims the text filters.</summary>
    public PokemonQuery Normalize() => this with
    {
        Page = Math.Max(Page, 1),
        PageSize = Math.Clamp(PageSize, 1, MaxPageSize),
        Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim().ToLowerInvariant(),
        Type = string.IsNullOrWhiteSpace(Type) ? null : Type.Trim().ToLowerInvariant(),
        Generation = Generation is >= 1 and <= 9 ? Generation : null
    };
}
