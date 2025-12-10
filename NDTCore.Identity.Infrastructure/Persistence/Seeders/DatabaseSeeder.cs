using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Seeders;

/// <summary>
/// Main database seeder that orchestrates all seeding operations
/// </summary>
public class DatabaseSeeder
{
    private readonly DefaultRolesSeeder _rolesSeeder;
    private readonly DefaultPermissionsSeeder _permissionsSeeder;
    private readonly DefaultUsersSeeder _usersSeeder;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        DefaultRolesSeeder rolesSeeder,
        DefaultPermissionsSeeder permissionsSeeder,
        DefaultUsersSeeder usersSeeder,
        ILogger<DatabaseSeeder> logger)
    {
        _rolesSeeder = rolesSeeder;
        _permissionsSeeder = permissionsSeeder;
        _usersSeeder = usersSeeder;
        _logger = logger;
    }

    /// <summary>
    /// Seed all default data in the correct order
    /// </summary>
    public async Task SeedAllAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Seed in the correct order (roles -> permissions -> users)
            await _rolesSeeder.SeedAsync();
            await _permissionsSeeder.SeedAsync();
            await _usersSeeder.SeedAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }
}

