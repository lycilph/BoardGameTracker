using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace BoardGameTracker.Client;

public static class Configure
{
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
}
