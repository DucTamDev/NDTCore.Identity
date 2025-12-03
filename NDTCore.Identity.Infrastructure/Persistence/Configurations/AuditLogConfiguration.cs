using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for AuditLog
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Table name
        builder.ToTable("AuditLogs");

        // Primary key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .IsRequired();

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.UserName)
            .HasMaxLength(256);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.HttpMethod)
            .HasMaxLength(10);

        builder.Property(a => a.RequestPath)
            .HasMaxLength(500);

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        // Indexes for better query performance
        builder.HasIndex(a => a.EntityType);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => new { a.EntityType, a.EntityId, a.CreatedAt });

        // CreatedAt default value
        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}

