using Microsoft.AspNetCore.Identity;

namespace NDTCore.Identity.Domain.Entities
{
    public class AppUserRole : IdentityUserRole<Guid>
    {
        public AppUser AppUser { get; set; } = default!;
        public AppRole AppRole { get; set; } = default!;
    }
}