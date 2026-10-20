using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pokedex.Application.Abstractions;
using Pokedex.Domain;

namespace Pokedex.Tests.Api;

/// <summary>
/// Boots the real API pipeline with the PokeAPI repository replaced by an in-memory fake.
/// </summary>
public sealed class PokemonApiFactory : WebApplicationFactory<Program>
{
    public FakePokemonRepository Repository { get; } = new();

    public PokemonApiFactory()
    {
        Seed(Repository);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPokemonRepository>();
            services.AddSingleton<IPokemonRepository>(Repository);
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
}
