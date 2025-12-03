using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NDTCore.Identity.API.HealthChecks;

/// <summary>
/// Health check for database connectivity
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the DatabaseHealthCheck class
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">The logger</param>
    public DatabaseHealthCheck(
        IdentityDbContext context,
        ILogger<DatabaseHealthCheck> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to connect to database
            await _context.Database.CanConnectAsync(cancellationToken);

            return HealthCheckResult.Healthy("Database connection is healthy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy(
                "Database connection failed",
                ex);
        }
    }
}
