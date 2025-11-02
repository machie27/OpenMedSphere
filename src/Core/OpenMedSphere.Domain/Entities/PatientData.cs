using OpenMedSphere.Domain.Primitives;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Domain.Entities;

/// <summary>
/// Represents patient data that can be used in research studies.
/// This is an aggregate root that encapsulates patient information and anonymization state.
/// </summary>
public sealed class PatientData : AggregateRoot<Guid>
{
    private const int MinYearOfBirth = 1900;

    /// <summary>
    /// Gets the anonymized patient identifier.
    /// </summary>
    public required PatientIdentifier PatientId { get; init; }

    /// <summary>
    /// Gets the year of birth (generalized for privacy).
    /// </summary>
    public int? YearOfBirth { get; set; }

    /// <summary>
    /// Gets the gender of the patient.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Gets the region or generalized location of the patient.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets the primary diagnosis or condition.
    /// </summary>
    public string? PrimaryDiagnosis { get; set; }

    /// <summary>
    /// Gets the list of secondary diagnoses.
    /// </summary>
    public List<string> SecondaryDiagnoses { get; init; } = [];

    /// <summary>
    /// Gets the list of medications.
    /// </summary>
    public List<string> Medications { get; init; } = [];

    /// <summary>
    /// Gets the clinical notes (anonymized).
    /// </summary>
    public string? ClinicalNotes { get; set; }

    /// <summary>
    /// Gets the ID of the anonymization policy applied to this data.
    /// </summary>
    public Guid? AnonymizationPolicyId { get; set; }

    /// <summary>
    /// Gets the date and time when the data was collected.
    /// </summary>
    public DateTime CollectedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the data was anonymized.
    /// </summary>
    public DateTime? AnonymizedAtUtc { get; set; }

    /// <summary>
    /// Gets a value indicating whether the data has been anonymized.
    /// </summary>
    public bool IsAnonymized { get; set; }

    /// <summary>
    /// Gets the date and time when the data was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the data was last updated.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private PatientData() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientData"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the patient data.</param>
    private PatientData(Guid id) : base(id)
    {
        CreatedAtUtc = DateTime.UtcNow;
        CollectedAtUtc = DateTime.UtcNow;
        IsAnonymized = false;
    }

    /// <summary>
    /// Creates a new patient data record.
    /// </summary>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns>A new patient data record.</returns>
    public static PatientData Create(PatientIdentifier patientId)
    {
        ArgumentNullException.ThrowIfNull(patientId);

        return new PatientData(Guid.NewGuid())
        {
            PatientId = patientId
        };
    }

    /// <summary>
    /// Updates the patient's demographic information.
    /// </summary>
    /// <param name="yearOfBirth">The year of birth.</param>
    /// <param name="gender">The gender.</param>
    /// <param name="region">The region or location.</param>
    public void UpdateDemographics(int? yearOfBirth, string? gender, string? region)
    {
        if (yearOfBirth.HasValue)
        {
            var currentYear = DateTime.UtcNow.Year;
            ArgumentOutOfRangeException.ThrowIfLessThan(yearOfBirth.Value, MinYearOfBirth);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(yearOfBirth.Value, currentYear);
        }

        YearOfBirth = yearOfBirth;
        Gender = gender;
        Region = region;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the primary diagnosis for the patient.
    /// </summary>
    /// <param name="diagnosis">The primary diagnosis.</param>
    public void SetPrimaryDiagnosis(string diagnosis)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnosis);

        PrimaryDiagnosis = diagnosis;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a secondary diagnosis to the patient record.
    /// </summary>
    /// <param name="diagnosis">The secondary diagnosis to add.</param>
    public void AddSecondaryDiagnosis(string diagnosis)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnosis);

        if (!SecondaryDiagnoses.Contains(diagnosis))
        {
            SecondaryDiagnoses.Add(diagnosis);
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Removes a secondary diagnosis from the patient record.
    /// </summary>
    /// <param name="diagnosis">The secondary diagnosis to remove.</param>
    public void RemoveSecondaryDiagnosis(string diagnosis)
    {
        if (SecondaryDiagnoses.Remove(diagnosis))
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Adds a medication to the patient record.
    /// </summary>
    /// <param name="medication">The medication to add.</param>
    public void AddMedication(string medication)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(medication);

        if (!Medications.Contains(medication))
        {
            Medications.Add(medication);
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Removes a medication from the patient record.
    /// </summary>
    /// <param name="medication">The medication to remove.</param>
    public void RemoveMedication(string medication)
    {
        if (Medications.Remove(medication))
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the clinical notes for the patient.
    /// </summary>
    /// <param name="notes">The clinical notes.</param>
    public void UpdateClinicalNotes(string? notes)
    {
        ClinicalNotes = notes;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the patient data as anonymized with the specified policy.
    /// </summary>
    /// <param name="policyId">The ID of the anonymization policy applied.</param>
    public void MarkAsAnonymized(Guid policyId)
    {
        AnonymizationPolicyId = policyId;
        AnonymizedAtUtc = DateTime.UtcNow;
        IsAnonymized = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the age of the patient (approximate, based on year of birth).
    /// </summary>
    /// <returns>The approximate age, or null if year of birth is not set.</returns>
    public int? GetApproximateAge() =>
        YearOfBirth.HasValue ? DateTime.UtcNow.Year - YearOfBirth.Value : null;
}
