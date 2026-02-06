using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="AnonymizationPolicy"/> entity.
/// </summary>
internal sealed class AnonymizationPolicyConfiguration : IEntityTypeConfiguration<AnonymizationPolicy>
{
    public void Configure(EntityTypeBuilder<AnonymizationPolicy> builder)
    {
        builder.ToTable("AnonymizationPolicies");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(2000);

        builder.Property(a => a.Level)
            .HasConversion(
                level => level.ToString(),
                value => Enum.Parse<AnonymizationLevel>(value))
            .HasMaxLength(50);

        builder.Property(a => a.GeneralizeDateOfBirth);
        builder.Property(a => a.GeneralizeLocation);
        builder.Property(a => a.SuppressRareDiagnoses);
        builder.Property(a => a.KAnonymityThreshold);
        builder.Property(a => a.IsActive);
        builder.Property(a => a.CreatedAtUtc);
        builder.Property(a => a.UpdatedAtUtc);

        builder.HasIndex(a => a.IsActive);

        builder.Ignore(a => a.DomainEvents);
    }
}
