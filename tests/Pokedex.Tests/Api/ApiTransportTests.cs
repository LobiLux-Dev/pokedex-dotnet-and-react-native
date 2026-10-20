using System.Net;
using System.Net.Http.Headers;

namespace Pokedex.Tests.Api;

/// <summary>
/// Verifies the transport level optimisations of the API.
/// </summary>
public sealed class ApiTransportTests : IClassFixture<PokemonApiFactory>
{
    private readonly PokemonApiFactory _factory;

    public ApiTransportTests(PokemonApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CatalogueResponses_AreCompressedWithGzip()
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/pokemon");
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        Assert.Contains("gzip", response.Content.Headers.ContentEncoding);
    }

    [Fact]
    public async Task CatalogueResponses_AreCompressedWithBrotli()
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/recipes");
        request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
        Assert.Contains("br", response.Content.Headers.ContentEncoding);
    }

    [Fact]
    public async Task CatalogueResponses_AreCacheable()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon");

        response.EnsureSuccessStatusCode();
        Assert.True(response.Headers.CacheControl?.Public);
        Assert.Equal(TimeSpan.FromMinutes(5), response.Headers.CacheControl?.MaxAge);
    }

    [Fact]
    public async Task NotFoundResponses_AreNotCacheable()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/pokemon/missingno");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotEqual(true, response.Headers.CacheControl?.Public);
    }

    [Fact]
    public async Task HealthEndpoint_DoesNotGetCatalogueCacheHeaders()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        Assert.NotEqual(true, response.Headers.CacheControl?.Public);
    }
}
