using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ResearchStudy"/> entity.
/// </summary>
internal sealed class ResearchStudyConfiguration : IEntityTypeConfiguration<ResearchStudy>
{
    public void Configure(EntityTypeBuilder<ResearchStudy> builder)
    {
        builder.ToTable("ResearchStudies");

        builder.HasKey(r => r.Id);

        builder.ComplexProperty(r => r.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("StudyCode")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.ComplexProperty(r => r.StudyPeriod, period =>
        {
            period.Property(p => p.Start)
                .HasColumnName("StudyPeriodStart")
                .IsRequired();
            period.Property(p => p.End)
                .HasColumnName("StudyPeriodEnd")
                .IsRequired();
        });

        builder.Property(r => r.Title).HasMaxLength(500).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(5000);
        builder.Property(r => r.PrincipalInvestigator).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Institution).HasMaxLength(300).IsRequired();
        builder.Property(r => r.ResearchArea).HasMaxLength(200);
        builder.Property(r => r.MaxParticipants);
        builder.Property(r => r.AnonymizationPolicyId);
        builder.Property(r => r.IsActive);
        builder.Property(r => r.CreatedAtUtc);
        builder.Property(r => r.UpdatedAtUtc);

        builder.Property<List<Guid>>("_patientDataIds")
            .HasColumnName("PatientDataIds")
            .HasColumnType("jsonb");

        builder.HasIndex(r => r.AnonymizationPolicyId);
        builder.HasIndex(r => r.ResearchArea);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex("StudyCode").IsUnique().HasDatabaseName("IX_ResearchStudies_StudyCode");

        builder.Ignore(r => r.DomainEvents);
        builder.Ignore(r => r.CurrentParticipantCount);
        builder.Ignore(r => r.PatientDataIds);
    }
}
