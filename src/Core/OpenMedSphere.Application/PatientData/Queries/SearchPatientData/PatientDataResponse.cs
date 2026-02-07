namespace OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

/// <summary>
/// Response DTO for patient data.
/// </summary>
public sealed record PatientDataResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the anonymized patient identifier.
    /// </summary>
    public required string PatientIdentifier { get; init; }

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
    public IReadOnlyList<string> SecondaryDiagnoses { get; init; } = [];

    /// <summary>
    /// Gets the medications.
    /// </summary>
    public IReadOnlyList<string> Medications { get; init; } = [];

    /// <summary>
    /// Gets whether the data is anonymized.
    /// </summary>
    public bool IsAnonymized { get; init; }

    /// <summary>
    /// Gets the collection date.
    /// </summary>
    public DateTime CollectedAtUtc { get; init; }

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }
}
