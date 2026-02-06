namespace OpenMedSphere.Application.ResearchStudies.Queries.SearchResearchStudies;

/// <summary>
/// Response DTO for research studies.
/// </summary>
public sealed record ResearchStudyResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the study code.
    /// </summary>
    public required string StudyCode { get; init; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the principal investigator.
    /// </summary>
    public required string PrincipalInvestigator { get; init; }

    /// <summary>
    /// Gets the institution.
    /// </summary>
    public required string Institution { get; init; }

    /// <summary>
    /// Gets the research area.
    /// </summary>
    public string? ResearchArea { get; init; }

    /// <summary>
    /// Gets the study period start.
    /// </summary>
    public DateTime StudyPeriodStart { get; init; }

    /// <summary>
    /// Gets the study period end.
    /// </summary>
    public DateTime StudyPeriodEnd { get; init; }

    /// <summary>
    /// Gets whether the study is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the current participant count.
    /// </summary>
    public int ParticipantCount { get; init; }

    /// <summary>
    /// Gets the maximum participants.
    /// </summary>
    public int? MaxParticipants { get; init; }

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }
}
