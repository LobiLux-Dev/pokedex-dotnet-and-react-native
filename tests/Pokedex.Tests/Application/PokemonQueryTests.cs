using Pokedex.Application.Abstractions;
using Pokedex.Application.Mapping;

namespace Pokedex.Tests.Application;

public sealed class PokemonQueryTests
{
    [Fact]
    public void Normalize_ClampsThePageNumber()
    {
        var query = new PokemonQuery { Page = 0 }.Normalize();

        Assert.Equal(1, query.Page);
    }

    [Fact]
    public void Normalize_ClampsThePageSize()
    {
        Assert.Equal(PokemonQuery.MaxPageSize, new PokemonQuery { PageSize = 5_000 }.Normalize().PageSize);
        Assert.Equal(1, new PokemonQuery { PageSize = 0 }.Normalize().PageSize);
    }

    [Fact]
    public void Normalize_TrimsAndLowersTheFilters()
    {
        var query = new PokemonQuery { Search = "  PIKA ", Type = " Electric " }.Normalize();

        Assert.Equal("pika", query.Search);
        Assert.Equal("electric", query.Type);
    }

    [Fact]
    public void Normalize_TurnsBlankFiltersIntoNull()
    {
        var query = new PokemonQuery { Search = "   ", Type = "" }.Normalize();

        Assert.Null(query.Search);
        Assert.Null(query.Type);
    }
}

public sealed class PokemonMapperTests
{
    [Theory]
    [InlineData("pikachu", "Pikachu")]
    [InlineData("mr-mime", "Mr Mime")]
    [InlineData("deoxys-normal", "Deoxys Normal")]
    [InlineData("nidoran-f", "Nidoran F")]
    public void ToDisplayName_ProducesAReadableName(string input, string expected)
    {
        Assert.Equal(expected, PokemonMapper.ToDisplayName(input));
    }

    [Fact]
    public void ToDisplayName_HandlesEmptyValues()
    {
        Assert.Equal(string.Empty, PokemonMapper.ToDisplayName("   "));
    }
}
