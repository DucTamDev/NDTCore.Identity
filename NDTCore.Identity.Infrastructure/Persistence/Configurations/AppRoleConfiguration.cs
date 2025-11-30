using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            // Table and key (inherited from IdentityRole)
            builder.ToTable("AspNetRoles");
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Name).IsRequired().HasMaxLength(256);
            builder.Property(r => r.NormalizedName).HasMaxLength(256);
            builder.Property(r => r.Description).HasMaxLength(500);
            builder.Property(r => r.CreatedBy).HasMaxLength(100);
            builder.Property(r => r.CreatedAt).HasColumnType("datetime");
        }
    }
}