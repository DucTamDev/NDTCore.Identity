using Microsoft.Extensions.Diagnostics.HealthChecks;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.API.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly NdtCoreIdentityDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(
            NdtCoreIdentityDbContext context,
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
}
