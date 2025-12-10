using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder for default system users
/// </summary>
public class DefaultUsersSeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DefaultUsersSeeder> _logger;

    public DefaultUsersSeeder(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IConfiguration configuration,
        ILogger<DefaultUsersSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding default users...");

        // Get default admin credentials from configuration
        var defaultAdminEmail = _configuration["SeedSettings:DefaultAdminEmail"] ?? "admin@ndtcore.com";
        var defaultAdminPassword = _configuration["SeedSettings:DefaultAdminPassword"] ?? "Admin@123456";

        // Check if admin user already exists
        var adminUser = await _userManager.FindByEmailAsync(defaultAdminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = "admin",
                Email = defaultAdminEmail,
                NormalizedUserName = "ADMIN",
                NormalizedEmail = defaultAdminEmail.ToUpperInvariant(),
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(adminUser, defaultAdminPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Created default admin user: {Email}", defaultAdminEmail);

                // Assign SuperAdmin role
                var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");
                if (superAdminRole != null)
                {
                    await _userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                    _logger.LogInformation("Assigned SuperAdmin role to {Email}", defaultAdminEmail);
                }
            }
            else
            {
                _logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            _logger.LogInformation("Admin user already exists, skipping");
        }

        _logger.LogInformation("Default users seeding completed");
    }
}

