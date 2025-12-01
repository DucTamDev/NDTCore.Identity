
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.API.HealthChecks
{
    public class IdentityHealthCheck : IHealthCheck
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<IdentityHealthCheck> _logger;

        public IdentityHealthCheck(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ILogger<IdentityHealthCheck> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if admin role exists
                var adminRoleExists = await _roleManager.RoleExistsAsync("Admin");
                if (!adminRoleExists)
                {
                    return HealthCheckResult.Degraded("Admin role not found");
                }

                // Check if at least one user exists
                var usersExist = _userManager.Users.Any();
                if (!usersExist)
                {
                    return HealthCheckResult.Degraded("No users found in the system");
                }

                return HealthCheckResult.Healthy("Identity system is healthy");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity health check failed");
                return HealthCheckResult.Unhealthy(
                    "Identity system check failed",
                    ex);
            }
        }
    }
}