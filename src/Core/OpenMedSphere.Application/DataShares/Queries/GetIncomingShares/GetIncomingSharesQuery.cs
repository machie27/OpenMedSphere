using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Queries.GetIncomingShares;

/// <summary>
/// Query to get incoming data shares for a recipient researcher.
/// </summary>
public sealed record GetIncomingSharesQuery : IQuery<IReadOnlyList<DataShareSummaryResponse>>
{
    /// <summary>
    /// Gets the recipient researcher's ID.
    /// </summary>
    public required Guid ResearcherId { get; init; }
}
