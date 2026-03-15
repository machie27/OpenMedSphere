using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Queries.GetOutgoingShares;

/// <summary>
/// Query to get outgoing data shares from a sender researcher.
/// </summary>
public sealed record GetOutgoingSharesQuery : IQuery<IReadOnlyList<DataShareSummaryResponse>>
{
    /// <summary>
    /// Gets the sender researcher's ID.
    /// </summary>
    public required Guid ResearcherId { get; init; }

    /// <summary>
    /// Gets the page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}
