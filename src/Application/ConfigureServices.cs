using BoardGameTracker.Application.Common.Behaviours;
using BoardGameTracker.Application.Common.Handlers;
using BoardGameTracker.Application.Identity;
using BoardGameTracker.Application.Identity.DTO;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameTracker.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationCommonServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        return services;
    }

    public static IServiceCollection AddApplicationClientServices(this IServiceCollection services)
    {
        services.AddApplicationCommonServices();

        services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();

        services.AddTransient<LoggingHandler>();
        services.AddTransient<AuthenticationHandler>();

        return services;
    }

    public static IServiceCollection AddApplicationServerServices(this IServiceCollection services)
    {
        services.AddApplicationCommonServices();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogUnhandledExceptionBehaviour<,>));

        return services;
    }
}
