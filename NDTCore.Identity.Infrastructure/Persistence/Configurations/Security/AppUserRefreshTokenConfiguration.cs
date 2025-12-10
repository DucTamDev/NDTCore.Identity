using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for AppUserRefreshToken
/// </summary>
public class AppUserRefreshTokenConfiguration : IEntityTypeConfiguration<AppUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<AppUserRefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);

        // Properties
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rt => rt.JwtId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(100);

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(100);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.JwtId);
        builder.HasIndex(rt => rt.UserId);
        builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt });

        // Relationships
        builder.HasOne(rt => rt.AppUser)
            .WithMany(u => u.AppUserRefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


