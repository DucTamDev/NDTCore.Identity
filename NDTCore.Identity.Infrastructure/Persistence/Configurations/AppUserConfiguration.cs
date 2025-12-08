using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // Table and key
            builder.ToTable("AspNetUsers");
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();

            // Properties
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Gender).HasMaxLength(50);
            builder.Property(u => u.Address).HasMaxLength(200);
            builder.Property(u => u.City).HasMaxLength(100);
            builder.Property(u => u.State).HasMaxLength(100);
            builder.Property(u => u.ZipCode).HasMaxLength(20);
            builder.Property(u => u.Country).HasMaxLength(100);
            builder.Property(u => u.AvatarUrl).HasMaxLength(500);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(256);
            builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
            builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
            builder.Property(u => u.PhoneNumber).HasMaxLength(50);

            // Indexes (inherited from IdentityUser)
            builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
            builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();

            // Additional useful indexes
            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            builder.HasIndex(u => u.IsDeleted)
                .HasDatabaseName("IX_Users_IsDeleted");

            builder.HasIndex(u => new { u.Email, u.IsDeleted })
                .HasDatabaseName("IX_Users_Email_IsDeleted");

            builder.HasIndex(u => u.LastLoginAt)
                .HasDatabaseName("IX_Users_LastLoginAt");

            builder.HasIndex(u => u.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");
        }
    }
}