using Microsoft.AspNetCore.Identity;

namespace NDTCore.Identity.Domain.Entities
{
    public class AppUserLogin : IdentityUserLogin<Guid>
    {
        public AppUser AppUser { get; set; } = default!;
    }
}