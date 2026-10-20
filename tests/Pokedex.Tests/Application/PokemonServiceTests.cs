using Pokedex.Application.Abstractions;
using Pokedex.Application.Dtos;
using Pokedex.Application.Services;
using Pokedex.Domain;
using Pokedex.Tests;

namespace Pokedex.Tests.Application;

public sealed class PokemonServiceTests
{
    private static (PokemonService Service, FakePokemonRepository Repository) CreateService()
    {
        var repository = new FakePokemonRepository();

        repository.Catalog.Add(new PokemonSummary(
            1, "bulbasaur", ["grass", "poison"], "sprite-1", "artwork-1"));
        repository.Catalog.Add(new PokemonSummary(
            4, "charmander", ["fire"], "sprite-4", "artwork-4"));
        repository.Catalog.Add(new PokemonSummary(
            7, "squirtle", ["water"], "sprite-7", "artwork-7"));
        repository.Catalog.Add(new PokemonSummary(
            25, "pikachu", ["electric"], "sprite-25", "artwork-25"));
        repository.Catalog.Add(new PokemonSummary(
            26, "raichu", ["electric"], "sprite-26", "artwork-26"));

        var service = new PokemonService(repository, Microsoft.Extensions.Logging.Abstractions.NullLogger<PokemonService>.Instance);
        return (service, repository);
    }

    [Fact]
    public async Task GetPokemonPageAsync_ReturnsTheRequestedPage()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Page = 2, PageSize = 2 },
            CancellationToken.None);

        Assert.Equal(2, page.Page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(5, page.TotalItems);
        Assert.Equal(3, page.TotalPages);
        Assert.True(page.HasNextPage);
        Assert.True(page.HasPreviousPage);
        Assert.Equal(["squirtle", "pikachu"], page.Items.Select(item => item.Name));
    }

    [Fact]
    public async Task GetPokemonPageAsync_FiltersByType()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Type = "electric" },
            CancellationToken.None);

        Assert.Equal(2, page.TotalItems);
        Assert.Equal(["pikachu", "raichu"], page.Items.Select(item => item.Name));
    }

    [Fact]
    public async Task GetPokemonPageAsync_MatchesAnywhereInTheName()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Search = "chu" },
            CancellationToken.None);

        Assert.Equal(2, page.TotalItems);
        Assert.Equal(new[] { "pikachu", "raichu" }, page.Items.Select(item => item.Name));
    }

    [Fact]
    public async Task GetPokemonPageAsync_MatchesTheMiddleOfTheName()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Search = "man" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("charmander", Assert.Single(page.Items).Name);
    }

    [Fact]
    public async Task GetPokemonPageAsync_MatchesAnywhereInTheId()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Search = "2" },
            CancellationToken.None);

        Assert.Equal(2, page.TotalItems);
        Assert.Equal(new[] { 25, 26 }, page.Items.Select(item => item.Id));
    }

    [Fact]
    public async Task GetPokemonPageAsync_FiltersByGeneration()
    {
        var (service, _) = CreateService();

        var generationOne = await service.GetPokemonPageAsync(
            new PokemonQuery { Generation = 1 },
            CancellationToken.None);

        var generationTwo = await service.GetPokemonPageAsync(
            new PokemonQuery { Generation = 2 },
            CancellationToken.None);

        Assert.Equal(5, generationOne.TotalItems);
        Assert.Equal(0, generationTwo.TotalItems);
    }

    [Fact]
    public async Task GetPokemonPageAsync_IgnoresOutOfRangeGenerations()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Generation = 42 },
            CancellationToken.None);

        Assert.Equal(5, page.TotalItems);
    }

    [Fact]
    public async Task GetPokemonPageAsync_CombinesSearchAndTypeFilters()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Search = "char", Type = "fire" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("charmander", Assert.Single(page.Items).Name);
    }

    [Fact]
    public async Task GetPokemonPageAsync_ClampsOutOfRangeValues()
    {
        var (service, _) = CreateService();

        var page = await service.GetPokemonPageAsync(
            new PokemonQuery { Page = -5, PageSize = 500 },
            CancellationToken.None);

        Assert.Equal(1, page.Page);
        Assert.Equal(PokemonQuery.MaxPageSize, page.PageSize);
        Assert.Equal(5, page.TotalItems);
    }

    [Fact]
    public async Task GetPokemonDetailAsync_ThrowsWhenThePokemonDoesNotExist()
    {
        var (service, _) = CreateService();

        await Assert.ThrowsAsync<Pokedex.Application.Common.PokemonNotFoundException>(
            () => service.GetPokemonDetailAsync("missingno", CancellationToken.None));
    }

    [Fact]
    public async Task GetPokemonDetailAsync_ReturnsStatsAbilitiesAndEvolution()
    {
        var (service, repository) = CreateService();

        repository.Pokemon["pikachu"] = new Pokemon(
            25,
            "pikachu",
            4,
            60,
            112,
            [new PokemonType("electric")],
            [new PokemonAbility("static", false)],
            [
                new PokemonStat("hp", 35, 0),
                new PokemonStat("attack", 55, 0)
            ],
            PokemonSprites.Empty,
            "pikachu",
            [new PokemonMove("thunder-shock", "electric", "special", 40, 100, 1, "level-up", "red-blue")]);

        repository.Species["pikachu"] = new PokemonSpecies(
            "pikachu", "Rata eléctrico", "Una rata amarilla.", "grassland", "yellow", 10, false, false);

        repository.EvolutionChains[10] = new EvolutionChain(
            10,
            new EvolutionNode("pichu", [], [new EvolutionNode("pikachu", [new EvolutionDetail("level-up", 1, null, null, null, null, null, null)], [])]));

        var detail = await service.GetPokemonDetailAsync("pikachu", CancellationToken.None);

        Assert.Equal(25, detail.Id);
        Assert.Equal("Pikachu", detail.DisplayName);
        Assert.Equal(["electric"], detail.Types);
        Assert.Equal(90, detail.TotalStats);
        Assert.Equal(6.0, detail.WeightInKg);
        Assert.Equal("Rata eléctrico", detail.Genus);
        Assert.Single(detail.Moves);
        Assert.Equal("thunder-shock", detail.Moves[0].Name);
        Assert.NotNull(detail.Evolution);
        Assert.Equal("pichu", detail.Evolution.Root.SpeciesName);
        Assert.Equal("pikachu", detail.Evolution.Root.EvolvesTo[0].SpeciesName);
    }

    [Fact]
    public async Task GetTypesAsync_MapsTheCatalogueTypes()
    {
        var (service, repository) = CreateService();
        repository.Types.Add(new PokemonTypeInfo("electric", 114));

        var types = await service.GetTypesAsync(CancellationToken.None);

        Assert.Equal("electric", Assert.Single(types).Name);
        Assert.Equal(114, types[0].PokemonCount);
    }
}
