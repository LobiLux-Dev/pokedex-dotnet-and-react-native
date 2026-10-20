using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pokedex.Application.Abstractions;
using Pokedex.Domain;
using Pokedex.Domain.Recipes;
using Pokedex.Domain.Seafood;

namespace Pokedex.Tests.Api;

/// <summary>
/// Boots the real API pipeline with the PokeAPI repository replaced by an in-memory fake.
/// </summary>
public sealed class PokemonApiFactory : WebApplicationFactory<Program>
{
    public FakePokemonRepository Repository { get; } = new();

    public FakeRecipeRepository Recipes { get; } = new();

    public FakeSeafoodRepository Seafood { get; } = new();

    public PokemonApiFactory()
    {
        Seed(Repository);
        SeedRecipes(Recipes);
        SeedSeafood(Seafood);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPokemonRepository>();
            services.AddSingleton<IPokemonRepository>(Repository);

            services.RemoveAll<IRecipeRepository>();
            services.AddSingleton<IRecipeRepository>(Recipes);

            services.RemoveAll<ISeafoodRepository>();
            services.AddSingleton<ISeafoodRepository>(Seafood);
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.BaseAddress = new Uri("http://localhost");
    }

    private static void Seed(FakePokemonRepository repository)
    {
        repository.Catalog.Add(new PokemonSummary(1, "bulbasaur", ["grass", "poison"], "sprite-1", "artwork-1"));
        repository.Catalog.Add(new PokemonSummary(4, "charmander", ["fire"], "sprite-4", "artwork-4"));
        repository.Catalog.Add(new PokemonSummary(25, "pikachu", ["electric"], "sprite-25", "artwork-25"));

        repository.Pokemon["pikachu"] = new Pokemon(
            25,
            "pikachu",
            4,
            60,
            112,
            [new PokemonType("electric")],
            [new PokemonAbility("static", false)],
            [new PokemonStat("hp", 35, 0)],
            PokemonSprites.Empty,
            "pikachu",
            []);

        repository.Types.Add(new PokemonTypeInfo("electric", 114));
    }

    private static void SeedRecipes(FakeRecipeRepository repository)
    {
        repository.Recipes.Add(new Recipe(
            1,
            "arroz-pikachu",
            "Arroz de Pikachu",
            "Resumen del arroz.",
            "Descripción del arroz.",
            "Plato principal",
            "Fácil",
            15,
            20,
            4,
            ["arroz"],
            ["gluten"],
            "Consejo.",
            [new RecipePokemon(25, "pikachu")],
            [new RecipeIngredient("Arroz", "300", "g", false)],
            [new RecipeStep(1, "Cocinar", "Al fuego.", 20)]));

        repository.Recipes.Add(new Recipe(
            2,
            "tarta-bulbasaur",
            "Tarta de Bulbasaur",
            "Resumen de la tarta.",
            "Descripción de la tarta.",
            "Postre",
            "Media",
            30,
            45,
            6,
            ["dulce"],
            ["huevo"],
            null,
            [new RecipePokemon(1, "bulbasaur")],
            [new RecipeIngredient("Harina", "250", "g", false)],
            [new RecipeStep(1, "Hornear", "Al horno.", 45)]));
    }

    private static void SeedSeafood(FakeSeafoodRepository repository)
    {
        repository.Entries.Add(new SeafoodEntry(
            1,
            "magikarp",
            "Magikarp",
            "Carpa",
            129,
            "magikarp",
            "Pescado",
            "Moderada",
            "Todo el año",
            "Origen.",
            "Preparación.",
            "Sabor.",
            "Maridaje.",
            110,
            18,
            4,
            25,
            "Consejo."));

        repository.Entries.Add(new SeafoodEntry(
            2,
            "corphish",
            "Corphish",
            "Langostino",
            341,
            "corphish",
            "Crustáceo",
            "Buena",
            "Verano",
            "Origen.",
            "Preparación.",
            "Sabor.",
            "Maridaje.",
            105,
            21,
            2,
            20,
            null));
    }
}
