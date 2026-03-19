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

        builder.HasOne<Researcher>()
            .WithMany()
            .HasForeignKey(d => d.SenderResearcherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Researcher>()
            .WithMany()
            .HasForeignKey(d => d.RecipientResearcherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PatientData>()
            .WithMany()
            .HasForeignKey(d => d.PatientDataId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.EncryptedPayload)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(d => d.EncapsulatedKey)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(d => d.Signature)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(d => d.SenderKeyVersion).IsRequired();
        builder.Property(d => d.RecipientKeyVersion).IsRequired();
        builder.Property(d => d.Status).IsRequired();
        builder.Property(d => d.SharedAtUtc);
        builder.Property(d => d.AccessedAtUtc);
        builder.Property(d => d.ExpiresAtUtc);
        builder.Property(d => d.CreatedAtUtc);
        builder.Property(d => d.UpdatedAtUtc);

        builder.Property<uint>("xmin")
            .IsRowVersion();

        builder.HasIndex(d => new { d.SenderResearcherId, d.SharedAtUtc })
            .HasDatabaseName("IX_DataShares_SenderResearcherId_SharedAtUtc");

        builder.HasIndex(d => new { d.RecipientResearcherId, d.SharedAtUtc })
            .HasDatabaseName("IX_DataShares_RecipientResearcherId_SharedAtUtc");

        builder.HasIndex(d => d.PatientDataId)
            .HasDatabaseName("IX_DataShares_PatientDataId");

        // Note: DataShareStatus.Expired is computed at query time (never persisted),
        // so this index covers Pending, Accepted, and Revoked statuses only.
        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_DataShares_Status");

        builder.Ignore(d => d.DomainEvents);
    }
}
