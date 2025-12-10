using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Identity.Managers;

/// <summary>
/// Custom UserManager implementation for application-specific user management logic
/// </summary>
public class ApplicationUserManager : UserManager<AppUser>
{
    public ApplicationUserManager(
        IUserStore<AppUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<AppUser> passwordHasher,
        IEnumerable<IUserValidator<AppUser>> userValidators,
        IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<AppUser>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }

    // Add custom user management methods here as needed
}

