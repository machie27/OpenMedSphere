using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for patient data.
/// </summary>
internal sealed class PatientDataRepository(ApplicationDbContext dbContext)
    : Repository<PatientData, Guid>(dbContext), IPatientDataRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<PatientData>> GetByDiagnosisAsync(
        string diagnosis,
        CancellationToken cancellationToken = default)
    {
        // Uses ToLower().Contains() instead of EF.Functions.ILike — EF Core 10 + Npgsql parameterizes
        // Contains() so user-supplied wildcards are safe, and both generate full-table scans anyway.
        var diagnosisLower = diagnosis.ToLower();

        return await DbSet
            .Where(p => p.PrimaryDiagnosis != null &&
                        p.PrimaryDiagnosis.ToLower().Contains(diagnosisLower))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PatientData>> GetAnonymizedAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(p => p.IsAnonymized)
            .ToListAsync(cancellationToken);
}
