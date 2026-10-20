using Pokedex.Application.Abstractions;
using Pokedex.Application.Common;
using Pokedex.Application.Services;
using Pokedex.Domain.Seafood;

namespace Pokedex.Tests.Application;

public sealed class SeafoodServiceTests
{
    private static (SeafoodService Service, FakeSeafoodRepository Repository) CreateService()
    {
        var repository = new FakeSeafoodRepository();

        repository.Entries.Add(BuildEntry(
            id: 1,
            slug: "magikarp",
            title: "Magikarp",
            realName: "Carpa",
            pokemonId: 129,
            pokemonName: "magikarp",
            category: "Pescado",
            sustainability: "Moderada",
            prep: 25));

        repository.Entries.Add(BuildEntry(
            id: 2,
            slug: "corphish",
            title: "Corphish",
            realName: "Langostino",
            pokemonId: 341,
            pokemonName: "corphish",
            category: "Crustáceo",
            sustainability: "Buena",
            prep: 15));

        repository.Entries.Add(BuildEntry(
            id: 3,
            slug: "cloyster",
            title: "Cloyster",
            realName: "Ostra",
            pokemonId: 91,
            pokemonName: "cloyster",
            category: "Molusco",
            sustainability: "Poco recomendada",
            prep: 45));

        return (new SeafoodService(repository), repository);
    }

    private static SeafoodEntry BuildEntry(
        int id,
        string slug,
        string title,
        string realName,
        int pokemonId,
        string pokemonName,
        string category,
        string sustainability,
        int prep) => new(
        id,
        slug,
        title,
        realName,
        pokemonId,
        pokemonName,
        category,
        sustainability,
        "Todo el año",
        "Origen de prueba.",
        "Preparación de prueba.",
        "Sabor de prueba.",
        "Maridaje de prueba.",
        120,
        18,
        4,
        prep,
        "Consejo de prueba.");

    [Fact]
    public async Task GetPageAsync_FiltersByCategory()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new SeafoodQuery { Category = "crustáceo" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("Corphish", Assert.Single(page.Items).Title);
    }

    [Fact]
    public async Task GetPageAsync_FiltersBySustainability()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new SeafoodQuery { Sustainability = "Buena" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("Corphish", Assert.Single(page.Items).Title);
    }

    [Fact]
    public async Task GetPageAsync_FiltersByPreparationTime()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new SeafoodQuery { MaxPrepMinutes = 20 },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
    }

    [Fact]
    public async Task GetPageAsync_SearchesInTheRealName()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new SeafoodQuery { Search = "ostra" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("Cloyster", Assert.Single(page.Items).Title);
    }

    [Fact]
    public async Task GetPageAsync_SearchesInThePokemonName()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new SeafoodQuery { Search = "magikarp" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
    }

    [Fact]
    public async Task GetDetailAsync_ReturnsTheEntryBySlug()
    {
        var (service, _) = CreateService();

        var detail = await service.GetDetailAsync("magikarp", CancellationToken.None);

        Assert.Equal("Carpa", detail.RealName);
        Assert.Equal(129, detail.PokemonId);
        Assert.Equal("Origen de prueba.", detail.Origin);
    }

    [Fact]
    public async Task GetDetailAsync_ThrowsWhenTheEntryDoesNotExist()
    {
        var (service, _) = CreateService();

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.GetDetailAsync("no-existe", CancellationToken.None));
    }

    [Fact]
    public async Task GetFacetsAsync_CountsTheFilterableValues()
    {
        var (service, _) = CreateService();

        var facets = await service.GetFacetsAsync(CancellationToken.None);

        Assert.Equal(3, facets.Categories.Count);
        Assert.Equal(3, facets.Sustainabilities.Count);
        Assert.Equal("Buena", facets.Sustainabilities[0].Name);
    }
}
