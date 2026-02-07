using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="PatientData"/> entity.
/// </summary>
internal sealed class PatientDataConfiguration : IEntityTypeConfiguration<PatientData>
{
    public void Configure(EntityTypeBuilder<PatientData> builder)
    {
        builder.ToTable("PatientData");

        builder.HasKey(p => p.Id);

        builder.ComplexProperty(p => p.PatientId, patientId =>
        {
            patientId.Property(pi => pi.Value)
                .HasColumnName("PatientIdentifier")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(p => p.YearOfBirth);
        builder.Property(p => p.Gender).HasMaxLength(50);
        builder.Property(p => p.Region).HasMaxLength(200);
        builder.Property(p => p.PrimaryDiagnosis).HasMaxLength(500);
        builder.Property(p => p.ClinicalNotes).HasMaxLength(10000);

        builder.Property<List<string>>("_secondaryDiagnoses")
            .HasColumnName("SecondaryDiagnoses")
            .HasColumnType("jsonb");

        builder.Property<List<string>>("_medications")
            .HasColumnName("Medications")
            .HasColumnType("jsonb");

        builder.OwnsOne(p => p.PrimaryDiagnosisCode, code =>
        {
            code.Property(c => c.Code)
                .HasColumnName("PrimaryDiagnosisCode_Code")
                .HasMaxLength(50);
            code.Property(c => c.DisplayName)
                .HasColumnName("PrimaryDiagnosisCode_DisplayName")
                .HasMaxLength(500);
            code.Property(c => c.CodingSystem)
                .HasColumnName("PrimaryDiagnosisCode_CodingSystem")
                .HasMaxLength(50);
            code.Property(c => c.EntityUri)
                .HasColumnName("PrimaryDiagnosisCode_EntityUri")
                .HasMaxLength(500);
        });

        builder.Property<List<MedicalCode>>("_secondaryDiagnosisCodes")
            .HasColumnName("SecondaryDiagnosisCodes")
            .HasColumnType("jsonb");

        builder.Property(p => p.IsAnonymized);
        builder.Property(p => p.AnonymizationPolicyId);
        builder.Property(p => p.CollectedAtUtc);
        builder.Property(p => p.AnonymizedAtUtc);
        builder.Property(p => p.CreatedAtUtc);
        builder.Property(p => p.UpdatedAtUtc);

        builder.HasIndex(p => p.PrimaryDiagnosis);
        builder.HasIndex(p => p.IsAnonymized);
        builder.HasIndex(p => p.CollectedAtUtc);
        builder.HasIndex(p => p.AnonymizationPolicyId);
        builder.HasIndex(p => p.Region);
        builder.HasIndex("PatientIdentifier").HasDatabaseName("IX_PatientData_PatientIdentifier");

        builder.Ignore(p => p.DomainEvents);
        builder.Ignore(p => p.SecondaryDiagnoses);
        builder.Ignore(p => p.SecondaryDiagnosisCodes);
        builder.Ignore(p => p.Medications);
    }
}
