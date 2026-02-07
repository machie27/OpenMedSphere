using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Commands.CreatePatientData;

/// <summary>
/// Command to create a new patient data record.
/// </summary>
public sealed record CreatePatientDataCommand : ICommand<Guid>
{
    /// <summary>
    /// Gets the year of birth.
    /// </summary>
    public int? YearOfBirth { get; init; }

    /// <summary>
    /// Gets the gender.
    /// </summary>
    public string? Gender { get; init; }

    /// <summary>
    /// Gets the region.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Gets the primary diagnosis.
    /// </summary>
    public string? PrimaryDiagnosis { get; init; }

    /// <summary>
    /// Gets the primary diagnosis ICD code.
    /// </summary>
    public string? PrimaryDiagnosisIcdCode { get; init; }

    /// <summary>
    /// Gets the secondary diagnoses.
    /// </summary>
    public List<string>? SecondaryDiagnoses { get; init; }

    /// <summary>
    /// Gets the medications.
    /// </summary>
    public List<string>? Medications { get; init; }

    /// <summary>
    /// Gets the clinical notes.
    /// </summary>
    public string? ClinicalNotes { get; init; }
}
