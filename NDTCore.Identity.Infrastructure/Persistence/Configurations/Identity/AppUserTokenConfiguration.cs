using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations
{
    public class AppUserTokenConfiguration : IEntityTypeConfiguration<AppUserToken>
    {
        public void Configure(EntityTypeBuilder<AppUserToken> builder)
        {
            // Table and key
            builder.ToTable("AspNetUserTokens");
            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            // Properties
            builder.Property(ut => ut.UserId).IsRequired();
            builder.Property(ut => ut.LoginProvider).IsRequired().HasMaxLength(128);
            builder.Property(ut => ut.Name).IsRequired().HasMaxLength(128);
            builder.Property(ut => ut.Value).HasMaxLength(512);

            // Relationship
            builder.HasOne(ut => ut.AppUser)
                   .WithMany(u => u.AppUserTokens)
                   .HasForeignKey(ut => ut.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
