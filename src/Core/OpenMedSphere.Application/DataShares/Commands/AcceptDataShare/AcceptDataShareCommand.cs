using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Commands.AcceptDataShare;

/// <summary>
/// Command to accept a data share as the recipient.
/// </summary>
public sealed record AcceptDataShareCommand : ICommand
{
    /// <summary>
    /// Gets the data share ID.
    /// </summary>
    public required Guid DataShareId { get; init; }

    /// <summary>
    /// Gets the researcher ID of the requester (must be the recipient).
    /// </summary>
    public required Guid ResearcherId { get; init; }
}
