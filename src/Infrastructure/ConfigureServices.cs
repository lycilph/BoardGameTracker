using Blazored.LocalStorage;
using Blazored.SessionStorage;
using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Identity.Storage;
using BoardGameTracker.Infrastructure.Config;
using BoardGameTracker.Infrastructure.Contracts;
using BoardGameTracker.Infrastructure.Identity.Services;
using BoardGameTracker.Infrastructure.Identity.Storage;
using BoardGameTracker.Infrastructure.Mail;
using BoardGameTracker.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameTracker.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureClientServices(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorage();
        services.AddBlazoredSessionStorage();

        services.AddScoped<IAuthenticationClient, AuthenticationClient>();
        services.AddScoped<ITokenStore, TokenStore>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServerServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<CosmosDBSettings>(configuration.GetSection(CosmosDBSettings.Key));
        services.Configure<IdentitySeedData>(configuration.GetSection(IdentitySeedData.Key));
        services.Configure<JWTSettings>(configuration.GetSection(JWTSettings.Key));
        services.Configure<MailOptions>(configuration.GetSection(MailOptions.Key));

        services.AddIdentity<ApplicationUser, ApplicationRole>(
            options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddUserStore<ApplicationUserStore>()
            .AddRoleStore<ApplicationRoleStore>()
            .AddDefaultTokenProviders();

        services.AddScoped<IMailSender, SmtpMailSender>(); 
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IIdentityContext, IdentityContext>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICosmosDBContainerFactory, CosmosDBContainerFactory>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
