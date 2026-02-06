using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence;

/// <summary>
/// The application database context.
/// </summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IUnitOfWork
{
    /// <summary>
    /// Gets the patient data set.
    /// </summary>
    public DbSet<PatientData> PatientData => Set<PatientData>();

    /// <summary>
    /// Gets the research studies set.
    /// </summary>
    public DbSet<ResearchStudy> ResearchStudies => Set<ResearchStudy>();

    /// <summary>
    /// Gets the anonymization policies set.
    /// </summary>
    public DbSet<AnonymizationPolicy> AnonymizationPolicies => Set<AnonymizationPolicy>();

    /// <summary>
    /// Gets the audit log entries set.
    /// </summary>
    public DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
