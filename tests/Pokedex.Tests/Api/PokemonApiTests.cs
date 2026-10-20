using System.Net;
using System.Text.Json;

namespace Pokedex.Tests.Api;

public sealed class PokemonApiTests : IClassFixture<PokemonApiFactory>
{
    private readonly PokemonApiFactory _factory;

    public PokemonApiTests(PokemonApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPokemonList_ReturnsTheCatalogue()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon?pageSize=10");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(3, root.GetProperty("totalItems").GetInt32());
        Assert.Equal("pikachu", root.GetProperty("items")[2].GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetPokemonList_FiltersByType()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon?type=fire");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, document.RootElement.GetProperty("totalItems").GetInt32());
    }

    [Fact]
    public async Task GetPokemonList_SearchesByName()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon?search=char");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, document.RootElement.GetProperty("totalItems").GetInt32());
    }

    [Fact]
    public async Task GetPokemonByName_ReturnsTheDetails()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon/pikachu");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(25, document.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("Pikachu", document.RootElement.GetProperty("displayName").GetString());
    }

    [Fact]
    public async Task GetPokemonByName_ReturnsNotFoundForUnknownNames()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon/missingno");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Pokémon not found", document.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task GetTypes_ReturnsTheTypes()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/types");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("electric", document.RootElement[0].GetProperty("name").GetString());
        Assert.Equal(114, document.RootElement[0].GetProperty("pokemonCount").GetInt32());
    }

    [Fact]
    public async Task HealthEndpoint_IsAvailable()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
