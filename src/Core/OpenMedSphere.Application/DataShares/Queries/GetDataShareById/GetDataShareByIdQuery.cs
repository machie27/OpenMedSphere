using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Queries.GetDataShareById;

/// <summary>
/// Query to get a data share by its identifier.
/// The requester must be either the sender or recipient.
/// </summary>
public sealed record GetDataShareByIdQuery : IQuery<DataShareResponse>
{
    /// <summary>
    /// Gets the data share ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the researcher ID of the requester (for authorization).
    /// </summary>
    public required Guid ResearcherId { get; init; }
}
