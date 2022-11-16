using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Application.Common.Handlers;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly;
using Polly.Caching;
using Polly.Extensions.Http;
using Polly.Registry;
using Refit;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System.Net;

namespace BoardGameTracker.Client;

public static class Configure
{
    private const string CachePolicyKey = "cache_policy";
    private const string RetryPolicyKey = "retry_policy";

    public static WebAssemblyHostBuilder ConfigureLogging(this WebAssemblyHostBuilder builder)
    {
        var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            .Enrich.FromLogContext()
            .WriteTo.BrowserHttp(controlLevelSwitch: levelSwitch, endpointUrl: builder.HostEnvironment.BaseAddress + "ingest")
            .WriteTo.BrowserConsole()
            .CreateLogger();
        builder.Logging.AddProvider(new SerilogLoggerProvider());

        return builder;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services, string base_address)
    {
        services
            .AddRefitClient<IAuthenticationClientInternal>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(base_address + "api/authentication"));

        services.AddHttpClient<IIdentityClient, IdentityClient>(
            c => c.BaseAddress = new Uri(base_address + "api/Identity/"))
            .AddHttpMessageHandler<AuthenticationHandler>();

        services.AddHttpClient<GameClient>(
            c => c.BaseAddress = new Uri(base_address + "api/game/"))
            .AddHttpMessageHandler<AuthenticationHandler>();

        services.AddHttpClient<BoardGameGeekClient>(
            c => c.BaseAddress = new Uri("https://boardgamegeek.com/xmlapi2/"))
            .AddHttpMessageHandler<LoggingHandler>()
            .AddPolicyHandlerFromRegistry(CachePolicyKey)
            .AddPolicyHandlerFromRegistry(RetryPolicyKey);

        return services;
    }

    public static WebAssemblyHost UsePolicies(this WebAssemblyHost host)
    {
        var cache_provider = host.Services.GetRequiredService<IAsyncCacheProvider>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var registry = host.Services.GetRequiredService<IPolicyRegistry<string>>();

        var cache_policy = Policy
            .CacheAsync<HttpResponseMessage>(
                cache_provider,
                new SlidingTtl(TimeSpan.FromSeconds(600)),
                onCachePut: (context, key) => logger.LogInformation("Caching '{key}'", key),
                onCacheGet: (context, key) => logger.LogInformation("Retrieving '{key}' from the cache", key),
                onCachePutError: (context, key, exception) => logger.LogWarning(exception, "Cannot add '{key}' to the cache", key),
                onCacheGetError: (context, key, exception) => logger.LogWarning(exception, "Cannot retrieve '{key}' from the cache", key),
                onCacheMiss: (context, key) => logger.LogInformation("Cache miss for '{key}'", key));
        registry.Add(CachePolicyKey, cache_policy);

        var retry_policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.Accepted)
        .WaitAndRetryAsync(3, retryAttempt =>
        {
            logger.LogInformation("Retrying request");
            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
        });
        registry.Add(RetryPolicyKey, retry_policy);

        return host;
    }
}
