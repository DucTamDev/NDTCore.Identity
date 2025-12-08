using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Application.Features.Authorization.Handlers;
using NDTCore.Identity.Contracts.Authorization.Permissions;
using NDTCore.Identity.Contracts.Authorization.Policies;
using NDTCore.Identity.Contracts.Configuration.Authorization;

namespace NDTCore.Identity.API.Configuration;

/// <summary>
/// Extension methods for configuring authorization services
/// </summary>
public static class AuthorizationConfiguration
{
    /// <summary>
    /// Adds authorization policies and handlers to the service collection
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        // Register permission registry as singleton
        services.AddSingleton<IPermissionRegistry, PermissionRegistry>();

        // Register policy builder
        services.AddSingleton<IPolicyBuilder, PolicyBuilder>();

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, HasAnyPermissionHandler>();
        services.AddScoped<IAuthorizationHandler, HasAllPermissionsHandler>();

        // Bind permission configuration from appsettings
        services.Configure<PermissionOptions>(configuration.GetSection("Authorization:Permissions"));

        // Add authorization without configuring policies yet
        services.AddAuthorization();

        // Add our configurator
        services.AddTransient<IConfigureOptions<AuthorizationOptions>, PermissionPolicyConfigurator>();

        return services;
    }
}