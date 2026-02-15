using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Researcher"/> entity.
/// </summary>
internal sealed class ResearcherConfiguration : IEntityTypeConfiguration<Researcher>
{
    public void Configure(EntityTypeBuilder<Researcher> builder)
    {
        builder.ToTable("Researchers");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(r => r.Institution)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(r => r.IsActive);
        builder.Property(r => r.CreatedAtUtc);
        builder.Property(r => r.UpdatedAtUtc);

        builder.OwnsOne(r => r.PublicKeys, keys =>
        {
            keys.Property(k => k.MlKemPublicKey)
                .HasColumnName("PublicKeys_MlKem")
                .IsRequired();

            keys.Property(k => k.MlDsaPublicKey)
                .HasColumnName("PublicKeys_MlDsa")
                .IsRequired();

            keys.Property(k => k.X25519PublicKey)
                .HasColumnName("PublicKeys_X25519")
                .IsRequired();

            keys.Property(k => k.EcdsaPublicKey)
                .HasColumnName("PublicKeys_Ecdsa")
                .IsRequired();

            keys.Property(k => k.KeyVersion)
                .HasColumnName("PublicKeys_KeyVersion")
                .IsRequired();
        });

        builder.Navigation(r => r.PublicKeys).IsRequired();

        builder.HasIndex(r => r.Email)
            .IsUnique()
            .HasDatabaseName("IX_Researchers_Email");

        builder.HasIndex(r => r.Institution)
            .HasDatabaseName("IX_Researchers_Institution");

        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("IX_Researchers_IsActive");

        builder.Ignore(r => r.DomainEvents);
    }
}
