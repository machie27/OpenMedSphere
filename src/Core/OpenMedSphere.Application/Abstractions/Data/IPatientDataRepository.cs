namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Repository interface for patient data.
/// </summary>
public interface IPatientDataRepository : IRepository<Domain.Entities.PatientData, Guid>
{
    /// <summary>
    /// Gets patient data by diagnosis text.
    /// </summary>
    /// <param name="diagnosis">The diagnosis to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Patient data matching the diagnosis.</returns>
    Task<IReadOnlyList<Domain.Entities.PatientData>> GetByDiagnosisAsync(string diagnosis, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all anonymized patient data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All anonymized patient data.</returns>
    Task<IReadOnlyList<Domain.Entities.PatientData>> GetAnonymizedAsync(CancellationToken cancellationToken = default);
}
