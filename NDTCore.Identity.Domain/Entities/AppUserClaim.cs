using Microsoft.AspNetCore.Identity;

namespace NDTCore.Identity.Domain.Entities;

public class AppUserClaim : IdentityUserClaim<Guid>
{
    public AppUser AppUser { get; set; } = default!;
}
