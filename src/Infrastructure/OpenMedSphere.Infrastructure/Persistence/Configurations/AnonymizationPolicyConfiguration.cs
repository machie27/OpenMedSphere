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

        SeedPredefinedPolicies(builder);
    }

    private static void SeedPredefinedPolicies(EntityTypeBuilder<AnonymizationPolicy> builder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "No Anonymization",
                Description = "No anonymization applied. Data remains in its original form.",
                Level = AnonymizationLevel.None,
                GeneralizeDateOfBirth = false,
                GeneralizeLocation = false,
                SuppressRareDiagnoses = false,
                KAnonymityThreshold = (int?)null,
                IsActive = true,
                CreatedAtUtc = seedDate,
                UpdatedAtUtc = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Basic Anonymization",
                Description = "Removes direct identifiers such as name, address, and SSN.",
                Level = AnonymizationLevel.Basic,
                GeneralizeDateOfBirth = false,
                GeneralizeLocation = false,
                SuppressRareDiagnoses = false,
                KAnonymityThreshold = (int?)null,
                IsActive = true,
                CreatedAtUtc = seedDate,
                UpdatedAtUtc = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Standard Anonymization",
                Description = "Removes direct identifiers and generalizes quasi-identifiers such as date of birth and location.",
                Level = AnonymizationLevel.Standard,
                GeneralizeDateOfBirth = true,
                GeneralizeLocation = true,
                SuppressRareDiagnoses = false,
                KAnonymityThreshold = (int?)null,
                IsActive = true,
                CreatedAtUtc = seedDate,
                UpdatedAtUtc = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Advanced Anonymization",
                Description = "Applies k-anonymity, generalization, and suppression techniques to prevent re-identification.",
                Level = AnonymizationLevel.Advanced,
                GeneralizeDateOfBirth = true,
                GeneralizeLocation = true,
                SuppressRareDiagnoses = true,
                KAnonymityThreshold = (int?)5,
                IsActive = true,
                CreatedAtUtc = seedDate,
                UpdatedAtUtc = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Name = "Full Anonymization",
                Description = "Applies differential privacy techniques and maximum generalization for the highest level of privacy protection.",
                Level = AnonymizationLevel.Full,
                GeneralizeDateOfBirth = true,
                GeneralizeLocation = true,
                SuppressRareDiagnoses = true,
                KAnonymityThreshold = (int?)5,
                IsActive = true,
                CreatedAtUtc = seedDate,
                UpdatedAtUtc = (DateTime?)null
            });
    }
}
