using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations
{
    public class AppUserClaimConfiguration : IEntityTypeConfiguration<AppUserClaim>
    {
        public void Configure(EntityTypeBuilder<AppUserClaim> builder)
        {
            // Table and key
            builder.ToTable("AspNetUserClaims");
            builder.HasKey(uc => uc.Id);

            // Properties
            builder.Property(uc => uc.UserId).IsRequired();
            builder.Property(uc => uc.ClaimType).HasMaxLength(256);
            builder.Property(uc => uc.ClaimValue).HasMaxLength(256);

            // Relationship
            builder.HasOne(uc => uc.AppUser)
                   .WithMany(u => u.AppUserClaims)
                   .HasForeignKey(uc => uc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}