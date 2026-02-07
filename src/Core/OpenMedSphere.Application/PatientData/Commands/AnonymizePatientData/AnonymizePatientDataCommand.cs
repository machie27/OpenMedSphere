using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Commands.AnonymizePatientData;

/// <summary>
/// Command to anonymize patient data with a specified policy.
/// </summary>
public sealed record AnonymizePatientDataCommand : ICommand
{
    /// <summary>
    /// Gets the patient data ID to anonymize.
    /// </summary>
    public required Guid PatientDataId { get; init; }

    /// <summary>
    /// Gets the anonymization policy ID to apply.
    /// </summary>
    public required Guid PolicyId { get; init; }
}
