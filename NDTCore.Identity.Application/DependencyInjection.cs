using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NDTCore.Identity.Application.Features.Authorization.Services;
using NDTCore.Identity.Contracts.Interfaces.Authorization;
using System.Reflection;

namespace NDTCore.Identity.Application;

/// <summary>
/// Dependency injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR with pipeline behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Register pipeline behaviors (order matters!)
            cfg.AddOpenBehavior(typeof(Common.Behaviors.LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(Common.Behaviors.ValidationBehavior<,>));
        });

        // FluentValidation - automatically discovers all validators in this assembly
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper
        services.AddAutoMapper(assembly);

        services.AddScoped<IUserPermissionService, UserPermissionService>();

        return services;
    }
}