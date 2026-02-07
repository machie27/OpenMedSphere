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
        string escapedDiagnosis = EscapeLikePattern(diagnosis);

        return await DbSet
            .Where(p => p.PrimaryDiagnosis != null &&
                        EF.Functions.ILike(p.PrimaryDiagnosis, $"%{escapedDiagnosis}%"))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PatientData>> GetAnonymizedAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(p => p.IsAnonymized)
            .ToListAsync(cancellationToken);

    private static string EscapeLikePattern(string input) =>
        input
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}
