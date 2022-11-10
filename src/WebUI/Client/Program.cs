using BoardGameTracker.Application;
using BoardGameTracker.Application.Common.Handlers;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Services;
using BoardGameTracker.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Refit;
using Serilog;
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

        builder.Services.AddHttpClient<DummyService>(
            client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/");
            })
            .AddHttpMessageHandler<LoggingHandler>()
            .AddHttpMessageHandler<AuthenticationHandler>();

        builder.Services
            .AddRefitClient<IAuthenticationClientInternal>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/authentication"))
            .AddHttpMessageHandler<LoggingHandler>();

        // Application services
        builder.Services.AddApplicationClientServices();
        builder.Services.AddInfrastructureClientServices();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        });

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        await builder.Build().RunAsync();
    }
}
