using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Contracts.Authorization;

namespace NDTCore.Identity.API.Configuration;

/// <summary>
/// Authorization configuration and policy setup
/// </summary>
public static class AuthorizationConfiguration
{
    /// <summary>
    /// Adds authorization policies to the service collection
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // User Management Policies
            options.AddPolicy("Users.View", policy =>
                policy.RequireClaim("Permission", Permissions.Users.View));
            
            options.AddPolicy("Users.Create", policy =>
                policy.RequireClaim("Permission", Permissions.Users.Create));
            
            options.AddPolicy("Users.Edit", policy =>
                policy.RequireClaim("Permission", Permissions.Users.Edit));
            
            options.AddPolicy("Users.Delete", policy =>
                policy.RequireClaim("Permission", Permissions.Users.Delete));

            options.AddPolicy("Users.Lock", policy =>
                policy.RequireClaim("Permission", Permissions.Users.Lock));

            options.AddPolicy("Users.Unlock", policy =>
                policy.RequireClaim("Permission", Permissions.Users.Unlock));

            options.AddPolicy("Users.ResetPassword", policy =>
                policy.RequireClaim("Permission", Permissions.Users.ResetPassword));

            options.AddPolicy("Users.ViewSensitiveData", policy =>
                policy.RequireClaim("Permission", Permissions.Users.ViewSensitiveData));

            // Role Management Policies
            options.AddPolicy("Roles.View", policy =>
                policy.RequireClaim("Permission", Permissions.Roles.View));
            
            options.AddPolicy("Roles.Create", policy =>
                policy.RequireClaim("Permission", Permissions.Roles.Create));
            
            options.AddPolicy("Roles.Edit", policy =>
                policy.RequireClaim("Permission", Permissions.Roles.Edit));
            
            options.AddPolicy("Roles.Delete", policy =>
                policy.RequireClaim("Permission", Permissions.Roles.Delete));

            options.AddPolicy("Roles.AssignToUser", policy =>
                policy.RequireClaim("Permission", Permissions.Roles.AssignToUser));

            options.AddPolicy("Roles.RemoveFromUser", policy =>
                policy.RequireClaim("Permission", Permissions.Roles.RemoveFromUser));

            // Role Claims Policies
            options.AddPolicy("RoleClaims.View", policy =>
                policy.RequireClaim("Permission", Permissions.RoleClaims.View));
            
            options.AddPolicy("RoleClaims.Create", policy =>
                policy.RequireClaim("Permission", Permissions.RoleClaims.Create));
            
            options.AddPolicy("RoleClaims.Delete", policy =>
                policy.RequireClaim("Permission", Permissions.RoleClaims.Delete));

            // System Administration Policies
            options.AddPolicy("SystemAdministration.ViewAuditLogs", policy =>
                policy.RequireClaim("Permission", Permissions.SystemAdministration.ViewAuditLogs));

            options.AddPolicy("SystemAdministration.ManageSystemSettings", policy =>
                policy.RequireClaim("Permission", Permissions.SystemAdministration.ManageSystemSettings));

            options.AddPolicy("SystemAdministration.ViewHealthChecks", policy =>
                policy.RequireClaim("Permission", Permissions.SystemAdministration.ViewHealthChecks));

            // Composite Policies (for convenience)
            // AdminOnly: Requires any admin permission
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "Permission" && (
                        c.Value == Permissions.Users.View ||
                        c.Value == Permissions.Users.Create ||
                        c.Value == Permissions.Users.Edit ||
                        c.Value == Permissions.Users.Delete ||
                        c.Value == Permissions.Roles.View ||
                        c.Value == Permissions.Roles.Create ||
                        c.Value == Permissions.Roles.Edit ||
                        c.Value == Permissions.Roles.Delete ||
                        c.Value == Permissions.SystemAdministration.ViewAuditLogs ||
                        c.Value == Permissions.SystemAdministration.ManageSystemSettings
                    ))));

            // UserManagement: Requires any user management permission
            options.AddPolicy("UserManagement", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "Permission" && (
                        c.Value == Permissions.Users.View ||
                        c.Value == Permissions.Users.Create ||
                        c.Value == Permissions.Users.Edit ||
                        c.Value == Permissions.Users.Delete ||
                        c.Value == Permissions.Users.Lock ||
                        c.Value == Permissions.Users.Unlock ||
                        c.Value == Permissions.Users.ResetPassword
                    ))));

            // RoleManagement: Requires any role management permission
            options.AddPolicy("RoleManagement", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "Permission" && (
                        c.Value == Permissions.Roles.View ||
                        c.Value == Permissions.Roles.Create ||
                        c.Value == Permissions.Roles.Edit ||
                        c.Value == Permissions.Roles.Delete ||
                        c.Value == Permissions.Roles.AssignToUser ||
                        c.Value == Permissions.Roles.RemoveFromUser
                    ))));
        });

        // Register permission-based authorization handler
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Permission requirement for authorization
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

