using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations
{
    public class AppUserLoginConfiguration : IEntityTypeConfiguration<AppUserLogin>
    {
        public void Configure(EntityTypeBuilder<AppUserLogin> builder)
        {
            // Table and key
            builder.ToTable("AspNetUserLogins");
            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            // Properties
            builder.Property(ul => ul.LoginProvider).IsRequired().HasMaxLength(128);
            builder.Property(ul => ul.ProviderKey).IsRequired().HasMaxLength(128);
            builder.Property(ul => ul.UserId).IsRequired();
            builder.Property(ul => ul.ProviderDisplayName).HasMaxLength(256);

            // Relationship
            builder.HasOne(ul => ul.AppUser)
                   .WithMany(u => u.AppUserLogins)
                   .HasForeignKey(ul => ul.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
