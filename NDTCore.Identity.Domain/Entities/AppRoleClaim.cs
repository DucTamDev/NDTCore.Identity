using Microsoft.AspNetCore.Identity;

namespace NDTCore.Identity.Domain.Entities
{
    public class AppRoleClaim : IdentityRoleClaim<Guid>
    {
        public AppRole AppRole { get; set; } = default!;

        public AppRoleClaim() { }

        public AppRoleClaim(Guid roleId, string claimType, string claimValue)
        {
            RoleId = roleId;
            ClaimType = claimType;
            ClaimValue = claimValue;
        }
    }
}
