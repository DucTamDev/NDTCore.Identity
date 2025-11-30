using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Domain.BaseEntities;

namespace NDTCore.Identity.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>, IAuditableEntity, IAggregateRoot
    {
        // === Profile Information ===
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();

        // === Contact Info ===
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }

        // === Account Status ===
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // === Audit Information ===
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // === Relationship ===
        public ICollection<AppUserClaim> AppUserClaims { get; set; } = new List<AppUserClaim>();
        public ICollection<AppUserLogin> AppUserLogins { get; set; } = new List<AppUserLogin>();
        public ICollection<AppUserToken> AppUserTokens { get; set; } = new List<AppUserToken>();
        public ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
    }
}