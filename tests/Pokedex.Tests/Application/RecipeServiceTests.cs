using Pokedex.Application.Abstractions;
using Pokedex.Application.Common;
using Pokedex.Application.Services;
using Pokedex.Domain.Recipes;

namespace Pokedex.Tests.Application;

public sealed class RecipeServiceTests
{
    private static (RecipeService Service, FakeRecipeRepository Repository) CreateService()
    {
        var repository = new FakeRecipeRepository();

        repository.Recipes.Add(BuildRecipe(
            id: 1,
            slug: "arroz-pikachu",
            title: "Arroz de Pikachu",
            category: "Plato principal",
            difficulty: "Fácil",
            prep: 15,
            cook: 20,
            tags: ["arroz", "vegetariano"],
            ingredients: ["Arroz redondo", "Caldo de verduras"],
            pokemon: (25, "pikachu")));

        repository.Recipes.Add(BuildRecipe(
            id: 2,
            slug: "tarta-bulbasaur",
            title: "Tarta de Bulbasaur",
            category: "Postre",
            difficulty: "Media",
            prep: 30,
            cook: 45,
            tags: ["dulce", "horno"],
            ingredients: ["Harina", "Mantequilla"],
            pokemon: (1, "bulbasaur")));

        repository.Recipes.Add(BuildRecipe(
            id: 3,
            slug: "curry-charmander",
            title: "Curry de Charmander",
            category: "Plato principal",
            difficulty: "Difícil",
            prep: 20,
            cook: 60,
            tags: ["picante", "arroz"],
            ingredients: ["Pollo", "Leche de coco"],
            pokemon: (4, "charmander")));

        return (new RecipeService(repository), repository);
    }

    private static Recipe BuildRecipe(
        int id,
        string slug,
        string title,
        string category,
        string difficulty,
        int prep,
        int cook,
        string[] tags,
        string[] ingredients,
        (int Id, string Name) pokemon) => new(
        id,
        slug,
        title,
        $"Resumen de {title}.",
        $"Descripción de {title}.",
        category,
        difficulty,
        prep,
        cook,
        4,
        tags,
        ["gluten"],
        "Consejo de la casa.",
        [new RecipePokemon(pokemon.Id, pokemon.Name)],
        ingredients.Select(name => new RecipeIngredient(name, "1", "unidad", false)).ToList(),
        [
            new RecipeStep(1, "Preparar", "Mezclar todo.", 5),
            new RecipeStep(2, "Cocinar", "Al fuego.", 10)
        ]);

    [Fact]
    public async Task GetPageAsync_ReturnsTheRequestedPage()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Page = 2, PageSize = 2 },
            CancellationToken.None);

        Assert.Equal(3, page.TotalItems);
        Assert.Equal(2, page.TotalPages);
        Assert.Single(page.Items);
    }

    [Fact]
    public async Task GetPageAsync_FiltersByCategory()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Category = "postre" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("Tarta de Bulbasaur", Assert.Single(page.Items).Title);
    }

    [Fact]
    public async Task GetPageAsync_FiltersByDifficulty()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Difficulty = "Fácil" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
    }

    [Fact]
    public async Task GetPageAsync_FiltersByTotalTime()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { MaxMinutes = 40 },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("Arroz de Pikachu", Assert.Single(page.Items).Title);
    }

    [Fact]
    public async Task GetPageAsync_FiltersByTag()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Tag = "horno" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
    }

    [Fact]
    public async Task GetPageAsync_SearchesInTheIngredients()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Search = "coco" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
        Assert.Equal("Curry de Charmander", Assert.Single(page.Items).Title);
    }

    [Fact]
    public async Task GetPageAsync_SearchesInThePokemon()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Search = "bulbasaur" },
            CancellationToken.None);

        Assert.Equal(1, page.TotalItems);
    }

    [Fact]
    public async Task GetPageAsync_ClampsOutOfRangeValues()
    {
        var (service, _) = CreateService();

        var page = await service.GetPageAsync(
            new RecipeQuery { Page = -3, PageSize = 900 },
            CancellationToken.None);

        Assert.Equal(1, page.Page);
        Assert.Equal(RecipeQuery.MaxPageSize, page.PageSize);
    }

    [Fact]
    public async Task GetDetailAsync_ReturnsTheRecipeBySlug()
    {
        var (service, _) = CreateService();

        var detail = await service.GetDetailAsync("arroz-pikachu", CancellationToken.None);

        Assert.Equal("Arroz de Pikachu", detail.Title);
        Assert.Equal(35, detail.TotalMinutes);
        Assert.Equal(2, detail.Ingredients.Count);
        Assert.Equal(2, detail.Steps.Count);
        Assert.Equal("Pikachu", detail.Pokemon[0].DisplayName);
    }

    [Fact]
    public async Task GetDetailAsync_ThrowsWhenTheRecipeDoesNotExist()
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

        Assert.Equal(2, facets.Categories.Count);
        Assert.Equal(3, facets.Difficulties.Count);
        Assert.Equal("Fácil", facets.Difficulties[0].Name);
        // arroz, vegetariano, dulce, horno y picante
        Assert.Equal(5, facets.Tags.Count);
    }
}
