using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.SearchResearchers;

/// <summary>
/// Query to search researchers by name, email, or institution.
/// </summary>
public sealed record SearchResearchersQuery : IQuery<IReadOnlyList<ResearcherSummaryResponse>>
{
    /// <summary>
    /// Gets the search query text.
    /// </summary>
    public required string Query { get; init; }
}
