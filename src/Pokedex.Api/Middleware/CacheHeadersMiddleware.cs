namespace Pokedex.Api.Middleware;

/// <summary>
/// Adds a short public cache lifetime to the GET endpoints of the catalogue.
/// The data is served from the server side cache and changes very rarely.
/// </summary>
public sealed class CacheHeadersMiddleware
{
    private const string CacheControlValue = "public, max-age=300";

    private readonly RequestDelegate _next;

    public CacheHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsGet(context.Request.Method)
            && context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == StatusCodes.Status200OK)
                {
                    context.Response.Headers.CacheControl = CacheControlValue;
                }

                return Task.CompletedTask;
            });
        }

        await _next(context);
    }
}
