using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="DataShare"/> entity.
/// </summary>
internal sealed class DataShareConfiguration : IEntityTypeConfiguration<DataShare>
{
    public void Configure(EntityTypeBuilder<DataShare> builder)
    {
        builder.ToTable("DataShares");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.SenderResearcherId).IsRequired();
        builder.Property(d => d.RecipientResearcherId).IsRequired();
        builder.Property(d => d.PatientDataId).IsRequired();

        builder.Property(d => d.EncryptedPayload)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(d => d.EncapsulatedKey)
            .IsRequired();

        builder.Property(d => d.Signature)
            .IsRequired();

        builder.Property(d => d.SenderKeyVersion).IsRequired();
        builder.Property(d => d.RecipientKeyVersion).IsRequired();
        builder.Property(d => d.Status).IsRequired();
        builder.Property(d => d.SharedAtUtc);
        builder.Property(d => d.AccessedAtUtc);
        builder.Property(d => d.ExpiresAtUtc);
        builder.Property(d => d.CreatedAtUtc);
        builder.Property(d => d.UpdatedAtUtc);

        builder.HasIndex(d => d.SenderResearcherId)
            .HasDatabaseName("IX_DataShares_SenderResearcherId");

        builder.HasIndex(d => d.RecipientResearcherId)
            .HasDatabaseName("IX_DataShares_RecipientResearcherId");

        builder.HasIndex(d => d.PatientDataId)
            .HasDatabaseName("IX_DataShares_PatientDataId");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_DataShares_Status");

        builder.Ignore(d => d.DomainEvents);
    }
}
