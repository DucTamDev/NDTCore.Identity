using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Identity.Stores;

/// <summary>
/// Custom user store implementation
/// </summary>
public class ApplicationUserStore : UserStore<AppUser, AppRole, Persistence.Context.IdentityDbContext, Guid, AppUserClaim, AppUserRole, AppUserLogin, AppUserToken, AppRoleClaim>
{
    public ApplicationUserStore(Persistence.Context.IdentityDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    // Add custom user store methods here as needed
}

