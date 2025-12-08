using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using NDTCore.Identity.Application.Features.Authentication.Services;
using NDTCore.Identity.Application.Features.Authorization.Services;
using NDTCore.Identity.Application.Features.Claims.Services;
using NDTCore.Identity.Application.Features.Permissions.Services;
using NDTCore.Identity.Application.Features.Roles.Services;
using NDTCore.Identity.Application.Features.UserRoles.Services;
using NDTCore.Identity.Application.Features.Users.Services;
using NDTCore.Identity.Application.Mappings;
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();

        // Register FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMappings();

        return services;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(MappingProfile).Assembly
        };

        services.AddAutoMapper(assemblies);
        return services;
    }
}