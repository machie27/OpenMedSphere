using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.ResearchStudies.Commands.CreateResearchStudy;

/// <summary>
/// Command to create a new research study.
/// </summary>
public sealed record CreateResearchStudyCommand : ICommand<Guid>
{
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
    /// Gets the study period start.
    /// </summary>
    public required DateTime StudyPeriodStart { get; init; }

    /// <summary>
    /// Gets the study period end.
    /// </summary>
    public required DateTime StudyPeriodEnd { get; init; }

    /// <summary>
    /// Gets the anonymization policy ID.
    /// </summary>
    public required Guid AnonymizationPolicyId { get; init; }

    /// <summary>
    /// Gets the research area.
    /// </summary>
    public string? ResearchArea { get; init; }

    /// <summary>
    /// Gets the maximum participants.
    /// </summary>
    public int? MaxParticipants { get; init; }
}
