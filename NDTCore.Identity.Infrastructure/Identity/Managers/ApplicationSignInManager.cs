using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Identity.Managers;

/// <summary>
/// Custom SignInManager implementation for application-specific sign-in logic
/// </summary>
public class ApplicationSignInManager : SignInManager<AppUser>
{
    public ApplicationSignInManager(
        UserManager<AppUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<AppUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<AppUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<AppUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    // Add custom sign-in methods here as needed
}

