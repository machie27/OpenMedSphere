using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence;

/// <summary>
/// The application database context.
/// </summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

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
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit. Call BeginTransactionAsync first.");
        }

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("No active transaction to roll back. Call BeginTransactionAsync first.");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        await base.DisposeAsync();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
