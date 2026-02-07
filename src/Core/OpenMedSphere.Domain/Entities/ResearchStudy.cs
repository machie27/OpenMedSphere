using OpenMedSphere.Domain.Events;
using OpenMedSphere.Domain.Primitives;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Domain.Entities;

/// <summary>
/// Represents a research study that uses anonymized patient data.
/// This is an aggregate root that manages study metadata, participants, and data access.
/// </summary>
public sealed class ResearchStudy : AggregateRoot<Guid>
{
    private readonly List<Guid> _patientDataIds = [];

    /// <summary>
    /// Gets the unique study code.
    /// </summary>
    public required StudyCode Code { get; init; }

    /// <summary>
    /// Gets the title of the research study.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets the description of the research study.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the principal investigator's name.
    /// </summary>
    public required string PrincipalInvestigator { get; set; }

    /// <summary>
    /// Gets the institution conducting the study.
    /// </summary>
    public required string Institution { get; set; }

    /// <summary>
    /// Gets the date range for the study.
    /// </summary>
    public required DateRange StudyPeriod { get; set; }

    /// <summary>
    /// Gets the ID of the anonymization policy required for this study.
    /// </summary>
    public required Guid AnonymizationPolicyId { get; set; }

    /// <summary>
    /// Gets the list of patient data IDs included in this study.
    /// </summary>
    public IReadOnlyCollection<Guid> PatientDataIds => _patientDataIds.AsReadOnly();

    /// <summary>
    /// Gets the maximum number of participants allowed in the study.
    /// </summary>
    public int? MaxParticipants { get; set; }

    /// <summary>
    /// Gets the current number of participants in the study.
    /// </summary>
    public int CurrentParticipantCount => _patientDataIds.Count;

    /// <summary>
    /// Gets the research area or field of study.
    /// </summary>
    public string? ResearchArea { get; set; }

    /// <summary>
    /// Gets a value indicating whether the study is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets the date and time when the study was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the study was last updated.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private ResearchStudy() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResearchStudy"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the study.</param>
    private ResearchStudy(Guid id) : base(id)
    {
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new research study.
    /// </summary>
    /// <param name="code">The study code.</param>
    /// <param name="title">The title of the study.</param>
    /// <param name="principalInvestigator">The principal investigator's name.</param>
    /// <param name="institution">The institution conducting the study.</param>
    /// <param name="studyPeriod">The date range for the study.</param>
    /// <param name="anonymizationPolicyId">The ID of the required anonymization policy.</param>
    /// <param name="description">The description of the study.</param>
    /// <returns>A new research study.</returns>
    public static ResearchStudy Create(
        StudyCode code,
        string title,
        string principalInvestigator,
        string institution,
        DateRange studyPeriod,
        Guid anonymizationPolicyId,
        string? description = null)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(principalInvestigator);
        ArgumentException.ThrowIfNullOrWhiteSpace(institution);
        ArgumentNullException.ThrowIfNull(studyPeriod);

        ResearchStudy study = new(Guid.CreateVersion7())
        {
            Code = code,
            Title = title,
            PrincipalInvestigator = principalInvestigator,
            Institution = institution,
            StudyPeriod = studyPeriod,
            AnonymizationPolicyId = anonymizationPolicyId,
            Description = description
        };

        study.RaiseDomainEvent(new ResearchStudyCreatedEvent(study.Id, code.Value));

        return study;
    }

    /// <summary>
    /// Updates the study information.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <param name="description">The new description.</param>
    /// <param name="principalInvestigator">The new principal investigator.</param>
    /// <param name="institution">The new institution.</param>
    public void UpdateInformation(
        string title,
        string? description,
        string principalInvestigator,
        string institution)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(principalInvestigator);
        ArgumentException.ThrowIfNullOrWhiteSpace(institution);

        Title = title;
        Description = description;
        PrincipalInvestigator = principalInvestigator;
        Institution = institution;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the study period.
    /// </summary>
    /// <param name="studyPeriod">The new study period.</param>
    public void UpdateStudyPeriod(DateRange studyPeriod)
    {
        ArgumentNullException.ThrowIfNull(studyPeriod);

        StudyPeriod = studyPeriod;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the maximum number of participants for the study.
    /// </summary>
    /// <param name="maxParticipants">The maximum number of participants.</param>
    public void SetMaxParticipants(int maxParticipants)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxParticipants, CurrentParticipantCount);

        MaxParticipants = maxParticipants;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the research area for the study.
    /// </summary>
    /// <param name="researchArea">The research area.</param>
    public void SetResearchArea(string researchArea)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(researchArea);

        ResearchArea = researchArea;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds patient data to the study.
    /// </summary>
    /// <param name="patientDataId">The ID of the patient data to add.</param>
    /// <exception cref="InvalidOperationException">Thrown when the study has reached maximum participants.</exception>
    public void AddPatientData(Guid patientDataId)
    {
        if (MaxParticipants.HasValue && CurrentParticipantCount >= MaxParticipants.Value)
        {
            throw new InvalidOperationException("Study has reached maximum number of participants.");
        }

        if (!_patientDataIds.Contains(patientDataId))
        {
            _patientDataIds.Add(patientDataId);
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Removes patient data from the study.
    /// </summary>
    /// <param name="patientDataId">The ID of the patient data to remove.</param>
    public void RemovePatientData(Guid patientDataId)
    {
        if (_patientDataIds.Remove(patientDataId))
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Activates the study.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the study.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the study can accept more participants.
    /// </summary>
    /// <returns>True if the study can accept more participants; otherwise, false.</returns>
    public bool CanAcceptParticipants() =>
        IsActive && (!MaxParticipants.HasValue || CurrentParticipantCount < MaxParticipants.Value);

    /// <summary>
    /// Checks if the study is currently ongoing.
    /// </summary>
    /// <returns>True if the study is ongoing; otherwise, false.</returns>
    public bool IsOngoing() => IsActive && StudyPeriod.Contains(DateTime.UtcNow);
}
