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

    /// <summary>
    /// Gets the page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}
