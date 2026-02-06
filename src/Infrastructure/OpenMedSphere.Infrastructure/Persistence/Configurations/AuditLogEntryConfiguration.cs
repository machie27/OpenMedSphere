using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="AuditLogEntry"/> entity.
/// </summary>
internal sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLog");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityType).HasMaxLength(200).IsRequired();
        builder.Property(a => a.EntityId).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Action).HasMaxLength(50).IsRequired();
        builder.Property(a => a.OldValues).HasColumnType("jsonb");
        builder.Property(a => a.NewValues).HasColumnType("jsonb");
        builder.Property(a => a.UserId).HasMaxLength(200);
        builder.Property(a => a.OccurredAtUtc);

        builder.HasIndex(a => a.EntityType);
        builder.HasIndex(a => a.OccurredAtUtc);
        builder.HasIndex(a => new { a.EntityType, a.EntityId });
    }
}
