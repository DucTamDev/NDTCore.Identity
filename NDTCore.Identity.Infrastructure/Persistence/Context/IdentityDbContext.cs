using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Configurations;

namespace NDTCore.Identity.Infrastructure.Persistence.Context;

/// <summary>
/// Identity database context
/// </summary>
public class IdentityDbContext : IdentityDbContext<
    AppUser,
    AppRole,
    Guid,
    AppUserClaim,
    AppUserRole,
    AppUserLogin,
    AppRoleClaim,
    AppUserToken>
{
    /// <summary>
    /// Initializes a new instance of the IdentityDbContext class
    /// </summary>
    /// <param name="options">The database context options</param>
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Audit logs for tracking all CRUD operations
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// Application permissions
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>
    /// Role-permission assignments
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <summary>
    /// Configures the model for the database context
    /// </summary>
    /// <param name="builder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply entity configurations
        builder.ApplyConfiguration(new AppUserConfiguration());
        builder.ApplyConfiguration(new AppRoleConfiguration());
        builder.ApplyConfiguration(new AppUserRoleConfiguration());
        builder.ApplyConfiguration(new AppUserClaimConfiguration());
        builder.ApplyConfiguration(new AppUserLoginConfiguration());
        builder.ApplyConfiguration(new AppUserTokenConfiguration());
        builder.ApplyConfiguration(new AppUserRefreshTokenConfiguration());
        builder.ApplyConfiguration(new AppRoleClaimConfiguration());
        builder.ApplyConfiguration(new AuditLogConfiguration());
        builder.ApplyConfiguration(new PermissionConfiguration());
        builder.ApplyConfiguration(new RolePermissionConfiguration());
    }
}

