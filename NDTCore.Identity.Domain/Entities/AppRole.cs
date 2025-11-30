using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Domain.BaseEntities;

namespace NDTCore.Identity.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>, IAuditableEntity, IAggregateRoot
    {
        public string? Description { get; set; }

        // === Audit Information ===
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
        public ICollection<AppRoleClaim> AppRoleClaims { get; set; } = new List<AppRoleClaim>();
    }
}