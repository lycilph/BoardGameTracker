using Serilog.Events;
using Serilog;

namespace BoardGameTracker.Server;

public static class Configure
{
    public static void CreateSeriLogger()
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error.WriteLine);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console() //For debug: outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
            .CreateLogger();
    }

    public static void UseSeriLogging(this WebApplication app)
    {
        app.UseSerilogIngestion();
        app.UseSerilogRequestLogging(options => options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information);
    }
}
