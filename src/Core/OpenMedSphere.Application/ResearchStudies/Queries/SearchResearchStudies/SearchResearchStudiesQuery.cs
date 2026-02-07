using OpenMedSphere.Application.Common;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.ResearchStudies.Queries.SearchResearchStudies;

/// <summary>
/// Query to search research studies with multiple criteria.
/// </summary>
public sealed record SearchResearchStudiesQuery : IQuery<PagedResult<ResearchStudyResponse>>
{
    /// <summary>
    /// Gets the research area to search for.
    /// </summary>
    public string? ResearchArea { get; init; }

    /// <summary>
    /// Gets the title text to search for.
    /// </summary>
    public string? TitleSearch { get; init; }

    /// <summary>
    /// Gets whether to filter by active status.
    /// </summary>
    public bool? ActiveOnly { get; init; }

    /// <summary>
    /// Gets the study period overlap start date.
    /// </summary>
    public DateTime? OverlapStart { get; init; }

    /// <summary>
    /// Gets the study period overlap end date.
    /// </summary>
    public DateTime? OverlapEnd { get; init; }

    /// <summary>
    /// Gets the page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}
