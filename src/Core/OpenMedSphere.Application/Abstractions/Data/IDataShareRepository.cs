using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Repository interface for data shares.
/// </summary>
public interface IDataShareRepository : IRepository<DataShare, Guid>
{
    /// <summary>
    /// Gets incoming data share summaries for a recipient researcher.
    /// Uses server-side projection to avoid loading the encrypted payload.
    /// </summary>
    /// <param name="recipientResearcherId">The recipient researcher's ID.</param>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Summary DTOs for shares sent to the researcher, ordered by most recent first.</returns>
    Task<IReadOnlyList<DataShareSummaryResponse>> GetIncomingSharesAsync(Guid recipientResearcherId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets outgoing data share summaries from a sender researcher.
    /// Uses server-side projection to avoid loading the encrypted payload.
    /// </summary>
    /// <param name="senderResearcherId">The sender researcher's ID.</param>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Summary DTOs for shares sent by the researcher, ordered by most recent first.</returns>
    Task<IReadOnlyList<DataShareSummaryResponse>> GetOutgoingSharesAsync(Guid senderResearcherId, int skip, int take, CancellationToken cancellationToken = default);
}
