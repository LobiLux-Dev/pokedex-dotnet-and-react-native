using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pokedex.Application.Abstractions;

namespace Pokedex.Infrastructure.Warmup;

/// <summary>
/// Warms the caches in the background right after the API starts, so the first user
/// request does not pay for the ~21 upstream calls needed to build the Pokémon catalogue.
/// </summary>
public sealed class CatalogWarmupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CatalogWarmupService> _logger;

    public CatalogWarmupService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<CatalogWarmupService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Let the host finish starting before doing any work.
        await Task.Yield();

        if (!_configuration.GetValue("Warmup:Enabled", true))
        {
            _logger.LogInformation("Cache warm-up is disabled.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        await WarmAsync(
            "recipes",
            cancellationToken => services.GetRequiredService<IRecipeRepository>()
                .GetAllAsync(cancellationToken),
            stoppingToken);

        await WarmAsync(
            "seafood",
            cancellationToken => services.GetRequiredService<ISeafoodRepository>()
                .GetAllAsync(cancellationToken),
            stoppingToken);

        await WarmAsync(
            "Pokémon catalogue",
            cancellationToken => services.GetRequiredService<IPokemonRepository>()
                .GetCatalogAsync(cancellationToken),
            stoppingToken);
    }

    private async Task WarmAsync<T>(
        string name,
        Func<CancellationToken, Task<T>> load,
        CancellationToken stoppingToken)
    {
        try
        {
            var startedAt = DateTimeOffset.UtcNow;
            await load(stoppingToken);
            _logger.LogInformation(
                "Cache warm-up finished for {Name} in {Elapsed} ms.",
                name,
                (DateTimeOffset.UtcNow - startedAt).TotalMilliseconds);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cache warm-up cancelled for {Name}.", name);
        }
        catch (Exception exception)
        {
            // A cold cache is not a fatal error: the first request will simply rebuild it.
            _logger.LogWarning(exception, "Cache warm-up failed for {Name}.", name);
        }
    }
}
