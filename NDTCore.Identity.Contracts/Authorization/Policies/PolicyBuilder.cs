using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Contracts.Authorization.Permissions;
using NDTCore.Identity.Contracts.Authorization.Requirements;

namespace NDTCore.Identity.Contracts.Authorization.Policies;

/// <summary>
/// Builds authorization policies from permission registry
/// </summary>
public sealed class PolicyBuilder : IPolicyBuilder
{
    private readonly IPermissionRegistry _permissionRegistry;

    public PolicyBuilder(IPermissionRegistry permissionRegistry)
    {
        _permissionRegistry = permissionRegistry;
    }

    public void ConfigurePolicies(AuthorizationOptions options)
    {
        // Register individual permission policies
        RegisterPermissionPolicies(options);

        // Register composite/named policies
        RegisterCompositePolicies(options);
    }

    private void RegisterPermissionPolicies(AuthorizationOptions options)
    {
        var allPermissions = _permissionRegistry.GetAllPermissions();

        foreach (var permission in allPermissions.Select(p => p.Name))
        {
            options.AddPolicy(permission, policy =>
                policy.Requirements.Add(new PermissionRequirement(permission)));
        }
    }

    private void RegisterCompositePolicies(AuthorizationOptions options)
    {
        // AdminOnly: Has any admin-level permission
        options.AddPolicy(ApplicationPolicies.AdminOnly, policy =>
        {
            var adminPermissions = new List<string>();

            // Get all permissions from Users module
            var usersPermissions = _permissionRegistry.GetModulePermissions("Users");
            adminPermissions.AddRange(usersPermissions.Select(p => p.Name));

            // Get all permissions from Roles module
            var rolesPermissions = _permissionRegistry.GetModulePermissions("Roles");
            adminPermissions.AddRange(rolesPermissions.Select(p => p.Name));

            // Get system administration permissions
            var sysAdminPermissions = _permissionRegistry.GetModulePermissions("SystemAdministration");
            adminPermissions.AddRange(sysAdminPermissions.Select(p => p.Name));

            if (adminPermissions.Any())
            {
                policy.Requirements.Add(new HasAnyPermissionRequirement(adminPermissions));
            }
        });

        // UserManagement: Has any user management permission
        options.AddPolicy(ApplicationPolicies.UserManagement, policy =>
        {
            var permissions = _permissionRegistry.GetModulePermissions("Users");
            if (permissions.Any())
            {
                policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
            }
        });

        // RoleManagement: Has any role management permission
        options.AddPolicy(ApplicationPolicies.RoleManagement, policy =>
        {
            var permissions = _permissionRegistry.GetModulePermissions("Roles");
            if (permissions.Any())
            {
                policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
            }
        });

        // SystemAdministration: Has any system admin permission
        options.AddPolicy(ApplicationPolicies.SystemAdministration, policy =>
        {
            var permissions = _permissionRegistry.GetModulePermissions("SystemAdministration");
            if (permissions.Any())
            {
                policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
            }
        });

        // AuthenticationManagement: Has any authentication permission
        options.AddPolicy(ApplicationPolicies.AuthenticationManagement, policy =>
        {
            var permissions = _permissionRegistry.GetModulePermissions("Authentication");
            if (permissions.Any())
            {
                policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
            }
        });
    }
}