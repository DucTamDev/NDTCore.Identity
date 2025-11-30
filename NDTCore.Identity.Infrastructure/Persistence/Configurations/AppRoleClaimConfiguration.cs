using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations
{
    public class AppRoleClaimConfiguration : IEntityTypeConfiguration<AppRoleClaim>
    {
        public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
        {
            // Table and key
            builder.ToTable("AspNetRoleClaims");
            builder.HasKey(rc => rc.Id);

            // Properties
            builder.Property(rc => rc.RoleId).IsRequired();
            builder.Property(rc => rc.ClaimType).HasMaxLength(256);
            builder.Property(rc => rc.ClaimValue).HasMaxLength(256);

            // Relationship
            builder.HasOne(rc => rc.AppRole)
                   .WithMany(r => r.AppRoleClaims)
                   .HasForeignKey(rc => rc.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}