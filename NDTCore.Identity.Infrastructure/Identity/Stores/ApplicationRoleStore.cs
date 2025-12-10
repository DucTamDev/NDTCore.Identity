using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Identity.Stores;

/// <summary>
/// Custom role store implementation
/// </summary>
public class ApplicationRoleStore : RoleStore<AppRole, Persistence.Context.IdentityDbContext, Guid, AppUserRole, AppRoleClaim>
{
    public ApplicationRoleStore(Persistence.Context.IdentityDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    // Add custom role store methods here as needed
}

