using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Commands.RevokeDataShare;

/// <summary>
/// Command to revoke a data share as the sender.
/// </summary>
public sealed record RevokeDataShareCommand : ICommand
{
    /// <summary>
    /// Gets the data share ID.
    /// </summary>
    public required Guid DataShareId { get; init; }

    /// <summary>
    /// Gets the researcher ID of the requester (must be the sender).
    /// </summary>
    public required Guid ResearcherId { get; init; }
}
