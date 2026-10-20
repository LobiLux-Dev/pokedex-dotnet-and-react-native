using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Pokedex.Application.Abstractions;
using Pokedex.Infrastructure.Options;
using Pokedex.Infrastructure.PokeApi;

namespace Pokedex.Infrastructure;

/// <summary>
/// Registers the infrastructure services (PokeAPI client, cache, resilience).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<PokeApiOptions>()
            .Bind(configuration.GetSection(PokeApiOptions.SectionName))
            .ValidateOnStart();

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(6)
            };
        });

        services
            .AddHttpClient<PokeApiClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<PokeApiOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromMilliseconds(500);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.UseJitter = true;

                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(45);

                // The sampling duration must be at least twice the attempt timeout.
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 10;
            });

        // The typed client is registered as transient by AddHttpClient; resolve it through the
        // interface so the repository lifetime stays scoped.
        services.AddScoped<IPokemonRepository>(serviceProvider =>
            serviceProvider.GetRequiredService<PokeApiClient>());

        return services;
    }
}
