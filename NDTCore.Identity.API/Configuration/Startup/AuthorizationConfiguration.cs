using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Application.Features.Authorization.Handlers;
using NDTCore.Identity.Application.Features.Authorization.Services;
using NDTCore.Identity.Contracts.Interfaces.Authorization;
using NDTCore.Identity.Contracts.Settings;

namespace NDTCore.Identity.API.Configuration.Startup;

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

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, HasAnyPermissionHandler>();
        services.AddScoped<IAuthorizationHandler, HasAllPermissionsHandler>();

        // Bind permission configuration from appsettings
        services.Configure<PermissionSettings>(configuration.GetSection("Authorization:Permissions"));

        // Register permission registry as singleton
        services.AddSingleton<IPermissionModuleRegistrar, PermissionModuleRegistrar>();

        // Add our provider
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}
