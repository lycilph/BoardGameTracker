using BoardGameTracker.Application;
using BoardGameTracker.Application.Authentication.Commands;
using BoardGameTracker.Infrastructure;
using BoardGameTracker.Infrastructure.Config;
using BoardGameTracker.Server;
using MediatR;
using Serilog;

namespace BoardGameTracker;

public class Program
{
    public static void Main(string[] args)
    {
        Configure.CreateSeriLogger();

        Log.ForContext<Program>().Information("Starting web host");

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddSwagger();
        builder.Services.AddApplicationServerServices();
        builder.Services.AddInfrastructureServerServices(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration.GetSection(JWTSettings.Key));

        // Easier to control what to include if done here (and not in ConfigureServices)
        builder.Services.AddMediatR(typeof(LoginCommand).Assembly);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.SeedIdentityData();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseSeriLogging();

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
