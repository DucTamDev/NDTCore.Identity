using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder for default system roles
/// </summary>
public class DefaultRolesSeeder
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<DefaultRolesSeeder> _logger;

    public DefaultRolesSeeder(
        RoleManager<AppRole> roleManager,
        ILogger<DefaultRolesSeeder> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding default roles...");

        var defaultRoles = new[]
        {
            new { Name = "SuperAdmin", Description = "Super Administrator with full system access", Priority = 1000 },
            new { Name = "Admin", Description = "Administrator with elevated privileges", Priority = 900 },
            new { Name = "Manager", Description = "Manager with user management capabilities", Priority = 500 },
            new { Name = "User", Description = "Standard user with basic access", Priority = 100 }
        };

        foreach (var roleData in defaultRoles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleData.Name);
            if (!roleExists)
            {
                var role = new AppRole
                {
                    Name = roleData.Name,
                    NormalizedName = roleData.Name.ToUpperInvariant(),
                    Description = roleData.Description,
                    Priority = roleData.Priority,
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role: {RoleName}", roleData.Name);
                }
                else
                {
                    _logger.LogError("Failed to create role {RoleName}: {Errors}",
                        roleData.Name,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("Role {RoleName} already exists, skipping", roleData.Name);
            }
        }

        _logger.LogInformation("Default roles seeding completed");
    }
}

