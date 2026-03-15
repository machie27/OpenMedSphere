using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Repository interface for data shares.
/// </summary>
public interface IDataShareRepository : IRepository<DataShare, Guid>
{
    /// <summary>
    /// Gets incoming data shares for a recipient researcher.
    /// </summary>
    /// <param name="recipientResearcherId">The recipient researcher's ID.</param>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Data shares sent to the researcher, ordered by most recent first.</returns>
    Task<IReadOnlyList<DataShare>> GetIncomingSharesAsync(Guid recipientResearcherId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets outgoing data shares from a sender researcher.
    /// </summary>
    /// <param name="senderResearcherId">The sender researcher's ID.</param>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Data shares sent by the researcher, ordered by most recent first.</returns>
    Task<IReadOnlyList<DataShare>> GetOutgoingSharesAsync(Guid senderResearcherId, int skip, int take, CancellationToken cancellationToken = default);
}
