using System.Net;
using System.Text.Json;

namespace Pokedex.Tests.Api;

public sealed class CatalogApiTests : IClassFixture<PokemonApiFactory>
{
    private readonly PokemonApiFactory _factory;

    public CatalogApiTests(PokemonApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetRecipes_ReturnsTheCatalogue()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(2, root.GetProperty("totalItems").GetInt32());
        Assert.Equal("Arroz de Pikachu", root.GetProperty("items")[0].GetProperty("title").GetString());
    }

    [Fact]
    public async Task GetRecipes_FiltersByCategory()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes?category=Postre");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, document.RootElement.GetProperty("totalItems").GetInt32());
    }

    [Fact]
    public async Task GetRecipes_SearchesInTheIngredients()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes?search=harina");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, document.RootElement.GetProperty("totalItems").GetInt32());
    }

    [Fact]
    public async Task GetRecipeFacets_ReturnsTheFilterValues()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes/facets");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2, document.RootElement.GetProperty("categories").GetArrayLength());
        Assert.Equal(2, document.RootElement.GetProperty("difficulties").GetArrayLength());
    }

    [Fact]
    public async Task GetRecipe_ReturnsTheDetails()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes/arroz-pikachu");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Arroz de Pikachu", document.RootElement.GetProperty("title").GetString());
        Assert.Equal(1, document.RootElement.GetProperty("ingredients").GetArrayLength());
        Assert.Equal("Pikachu", document.RootElement.GetProperty("pokemon")[0].GetProperty("displayName").GetString());
    }

    [Fact]
    public async Task GetRecipe_ReturnsNotFoundForUnknownRecipes()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/recipes/no-existe");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSeafood_ReturnsTheCatalogue()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/seafood");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2, document.RootElement.GetProperty("totalItems").GetInt32());
    }

    [Fact]
    public async Task GetSeafood_FiltersBySustainability()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/seafood?sustainability=Buena");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, document.RootElement.GetProperty("totalItems").GetInt32());
    }

    [Fact]
    public async Task GetSeafoodFacets_ReturnsTheFilterValues()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/seafood/facets");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2, document.RootElement.GetProperty("categories").GetArrayLength());
        Assert.Equal(2, document.RootElement.GetProperty("sustainabilities").GetArrayLength());
    }

    [Fact]
    public async Task GetSeafoodEntry_ReturnsTheDetails()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/seafood/magikarp");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Carpa", document.RootElement.GetProperty("realName").GetString());
        Assert.Equal(129, document.RootElement.GetProperty("pokemonId").GetInt32());
    }

    [Fact]
    public async Task GetSeafoodEntry_ReturnsNotFoundForUnknownEntries()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/seafood/no-existe");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
