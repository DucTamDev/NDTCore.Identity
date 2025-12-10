using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants.Authorization;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder for default system permissions
/// </summary>
public class DefaultPermissionsSeeder
{
    private readonly IdentityDbContext _context;
    private readonly IPermissionRepository _permissionRepository;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<DefaultPermissionsSeeder> _logger;

    public DefaultPermissionsSeeder(
        IdentityDbContext context,
        IPermissionRepository permissionRepository,
        RoleManager<AppRole> roleManager,
        ILogger<DefaultPermissionsSeeder> logger)
    {
        _context = context;
        _permissionRepository = permissionRepository;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding default permissions...");

        // Define all permissions
        var allPermissions = new List<(string Name, string Description, string Module)>
        {
            // User Management Permissions
            (PermissionNames.Users.View, "View users", "Users"),
            (PermissionNames.Users.Create, "Create users", "Users"),
            (PermissionNames.Users.Edit, "Edit users", "Users"),
            (PermissionNames.Users.Delete, "Delete users", "Users"),
            (PermissionNames.Users.Lock, "Lock users", "Users"),
            (PermissionNames.Users.Unlock, "Unlock users", "Users"),
            (PermissionNames.Users.ResetPassword, "Reset user passwords", "Users"),
            (PermissionNames.Users.ViewSensitiveData, "View user sensitive data", "Users"),
            
            // Role Management Permissions
            (PermissionNames.Roles.View, "View roles", "Roles"),
            (PermissionNames.Roles.Create, "Create roles", "Roles"),
            (PermissionNames.Roles.Edit, "Edit roles", "Roles"),
            (PermissionNames.Roles.Delete, "Delete roles", "Roles"),
            (PermissionNames.Roles.AssignToUser, "Assign roles to users", "Roles"),
            (PermissionNames.Roles.RemoveFromUser, "Remove roles from users", "Roles"),
            
            // Role Claims Management
            (PermissionNames.RoleClaims.View, "View role claims", "RoleClaims"),
            (PermissionNames.RoleClaims.Create, "Create role claims", "RoleClaims"),
            (PermissionNames.RoleClaims.Delete, "Delete role claims", "RoleClaims"),
            
            // Authentication
            (PermissionNames.Authentication.Login, "User login", "Authentication"),
            (PermissionNames.Authentication.RefreshToken, "Refresh tokens", "Authentication"),
            (PermissionNames.Authentication.RevokeToken, "Revoke tokens", "Authentication"),
            (PermissionNames.Authentication.ViewTokens, "View tokens", "Authentication"),
            
            // System Administration
            (PermissionNames.SystemAdministration.ViewAuditLogs, "View audit logs", "SystemAdministration"),
            (PermissionNames.SystemAdministration.ManageSystemSettings, "Manage system settings", "SystemAdministration"),
            (PermissionNames.SystemAdministration.ViewHealthChecks, "View health checks", "SystemAdministration"),
        };

        // Seed permissions
        foreach (var (name, description, module) in allPermissions)
        {
            var existingPermission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == name);

            if (existingPermission == null)
            {
                var permission = new Permission
                {
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Permissions.Add(permission);
                _logger.LogInformation("Created permission: {PermissionName}", name);
            }
        }

        await _context.SaveChangesAsync();

        // Assign all permissions to SuperAdmin role
        await AssignPermissionsToSuperAdmin();

        _logger.LogInformation("Default permissions seeding completed");
    }

    private async Task AssignPermissionsToSuperAdmin()
    {
        var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");
        if (superAdminRole == null)
        {
            _logger.LogWarning("SuperAdmin role not found, skipping permission assignment");
            return;
        }

        var allPermissions = await _context.Permissions.ToListAsync();
        var existingRolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == superAdminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        foreach (var permission in allPermissions)
        {
            if (!existingRolePermissions.Contains(permission.Id))
            {
                var rolePermission = new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.RolePermissions.Add(rolePermission);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Assigned all permissions to SuperAdmin role");
    }
}

