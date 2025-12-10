using Microsoft.AspNetCore.Identity;

namespace NDTCore.Identity.Domain.Entities
{
    public class AppUserToken : IdentityUserToken<Guid>
    {
        public AppUser AppUser { get; set; } = default!;
    }
}
