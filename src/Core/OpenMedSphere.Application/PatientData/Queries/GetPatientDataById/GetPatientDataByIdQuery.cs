using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

namespace OpenMedSphere.Application.PatientData.Queries.GetPatientDataById;

/// <summary>
/// Query to get patient data by its identifier.
/// </summary>
public sealed record GetPatientDataByIdQuery : IQuery<PatientDataResponse>
{
    /// <summary>
    /// Gets the patient data ID.
    /// </summary>
    public required Guid Id { get; init; }
}
