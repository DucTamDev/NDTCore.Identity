using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Identity.Managers;

/// <summary>
/// Custom RoleManager implementation for application-specific role management logic
/// </summary>
public class ApplicationRoleManager : RoleManager<AppRole>
{
    public ApplicationRoleManager(
        IRoleStore<AppRole> store,
        IEnumerable<IRoleValidator<AppRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<RoleManager<AppRole>> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }

    // Add custom role management methods here as needed
}

