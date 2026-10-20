namespace Pokedex.Infrastructure.Options;

/// <summary>
/// Configuration of the upstream PokeAPI integration.
/// </summary>
public sealed class PokeApiOptions
{
    public const string SectionName = "PokeApi";

    /// <summary>Base address of the PokeAPI REST endpoints.</summary>
    public string BaseUrl { get; set; } = "https://pokeapi.co/api/v2/";

    /// <summary>User agent sent on every outbound request.</summary>
    public string UserAgent { get; set; } = "PokedexApi/1.0 (+https://github.com/)";

    /// <summary>Base address of the official sprite images (small sprites).</summary>
    public string SpriteBaseUrl { get; set; } =
        "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon";

    /// <summary>Base address of the official artwork images.</summary>
    public string ArtworkBaseUrl { get; set; } =
        "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork";

    /// <summary>How long the Pokémon catalogue stays cached.</summary>
    public TimeSpan CatalogCacheDuration { get; set; } = TimeSpan.FromHours(12);

    /// <summary>How long an individual Pokémon detail stays cached.</summary>
    public TimeSpan DetailCacheDuration { get; set; } = TimeSpan.FromHours(6);

    /// <summary>How long move details stay cached.</summary>
    public TimeSpan MoveCacheDuration { get; set; } = TimeSpan.FromDays(7);

    /// <summary>Maximum number of parallel requests issued against PokeAPI.</summary>
    public int MaxConcurrency { get; set; } = 6;
}
