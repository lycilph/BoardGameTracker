using BoardGameTracker.Application;
using BoardGameTracker.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Polly.Caching.Memory;
using Polly.Caching;
using System.Reflection;

namespace BoardGameTracker.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.ConfigureLogging();

        // There is a bug right now in the Serilog.Sinks.BrowserConsole package (see issue here: https://github.com/serilog/serilog-sinks-browserconsole/issues/20)
        //Log.ForContext<Program>().Information("Starting web app");

        // Application services
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();
        builder.Services.AddPolicyRegistry();
        builder.Services.AddHttpClients(builder.HostEnvironment.BaseAddress);
        builder.Services.AddApplicationClientServices();
        builder.Services.AddInfrastructureClientServices();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        });

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        var host = builder.Build();

        host.UsePolicies();

        await host.RunAsync();
    }
}
