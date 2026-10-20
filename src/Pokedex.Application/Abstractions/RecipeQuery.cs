namespace Pokedex.Application.Abstractions;

/// <summary>
/// Query parameters of the recipe catalogue.
/// </summary>
public sealed record RecipeQuery
{
    public const int DefaultPageSize = 12;
    public const int MaxPageSize = 50;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>Free text search over the title, the summary, the tags and the ingredients.</summary>
    public string? Search { get; init; }

    public string? Category { get; init; }

    public string? Difficulty { get; init; }

    /// <summary>Keeps only the recipes that take this many minutes or less.</summary>
    public int? MaxMinutes { get; init; }

    public string? Tag { get; init; }

    public RecipeQuery Normalize() => this with
    {
        Page = Math.Max(Page, 1),
        PageSize = Math.Clamp(PageSize, 1, MaxPageSize),
        Search = string.IsNullOrWhiteSpace(Search) ? null : Search.Trim().ToLowerInvariant(),
        Category = string.IsNullOrWhiteSpace(Category) ? null : Category.Trim().ToLowerInvariant(),
        Difficulty = string.IsNullOrWhiteSpace(Difficulty) ? null : Difficulty.Trim().ToLowerInvariant(),
        Tag = string.IsNullOrWhiteSpace(Tag) ? null : Tag.Trim().ToLowerInvariant(),
        MaxMinutes = MaxMinutes is <= 0 ? null : MaxMinutes
    };
}
