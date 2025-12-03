using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using NDTCore.Identity.Application.Features.Authentication.Services;
using NDTCore.Identity.Application.Features.Roles.Services;
using NDTCore.Identity.Application.Features.Users.Services;
using NDTCore.Identity.Application.Services;
using NDTCore.Identity.Contracts.Interfaces.Services;
using System.Reflection;

namespace NDTCore.Identity.Application;

/// <summary>
/// Extension methods for configuring application services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IAuthService, Features.Authentication.Services.AuthenticationService>();
        services.AddScoped<IUserService, Features.Users.Services.UserService>();
        services.AddScoped<IRoleService, Features.Roles.Services.RoleService>();
        services.AddScoped<IUserRoleService, Features.UserRoles.Services.UserRoleService>();
        services.AddScoped<IClaimService, Features.Claims.Services.ClaimService>();
        services.AddScoped<IPermissionService, Features.Permissions.Services.PermissionService>();
        services.AddScoped<IJwtTokenService, Services.JwtTokenService>();

        // Register FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}

