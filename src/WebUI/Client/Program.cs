using BoardGameTracker.Application;
using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Application.Common.Handlers;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Services;
using BoardGameTracker.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Polly.Caching.Memory;
using Polly.Caching;
using Refit;
using System.Reflection;
using Polly.Extensions.Http;
using Polly;
using System.Net;

namespace BoardGameTracker.Client;

public class Program
{
    private const string CachePolicyKey = "cache_policy";
    private const string RetryPolicyKey = "retry_policy";

    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.ConfigureLogging();

        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();

        var registry = builder.Services.AddPolicyRegistry();

        // There is a bug right now in the Serilog.Sinks.BrowserConsole package (see issue here: https://github.com/serilog/serilog-sinks-browserconsole/issues/20)
        //Log.ForContext<Program>().Information("Starting web app");

        builder.Services.AddHttpClient<DummyService>(
            c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/"))
            .AddHttpMessageHandler<LoggingHandler>()
            .AddHttpMessageHandler<AuthenticationHandler>();

        builder.Services
            .AddRefitClient<IAuthenticationClientInternal>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/authentication"))
            .AddHttpMessageHandler<LoggingHandler>();

        builder.Services.AddHttpClient<BoardGameGeekClient>(
            c => c.BaseAddress = new Uri("https://boardgamegeek.com/xmlapi2/"))
            .AddHttpMessageHandler<LoggingHandler>()
            .AddPolicyHandlerFromRegistry(CachePolicyKey)
            .AddPolicyHandlerFromRegistry(RetryPolicyKey);

        // Application services
        builder.Services.AddApplicationClientServices();
        builder.Services.AddInfrastructureClientServices();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        });

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        var host = builder.Build();

        var cache_provider = host.Services.GetRequiredService<IAsyncCacheProvider>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var cache_policy = Policy
            .CacheAsync<HttpResponseMessage>(
                cache_provider,
                new SlidingTtl(TimeSpan.FromSeconds(60)),
                onCachePut: (context, key) => logger.LogInformation($"Caching '{key}'."),
                onCacheGet: (context, key) => logger.LogInformation($"Retrieving '{key}' from the cache."),
                onCachePutError: (context, key, exception) => logger.LogWarning(exception, $"Cannot add '{key}' to the cache."),
                onCacheGetError: (context, key, exception) => logger.LogWarning(exception, $"Cannot retrieve '{key}' from the cache."),
                onCacheMiss: (context, key) => logger.LogInformation($"Cache miss for '{key}'."));
        registry.Add(CachePolicyKey, cache_policy);

        var retry_policy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.Accepted)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        registry.Add(RetryPolicyKey, retry_policy);


        await host.RunAsync();
    }
}
