using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when a data share is accessed (accepted) by the recipient.
/// </summary>
public sealed record DataShareAccessedEvent(
    Guid DataShareId,
    Guid RecipientResearcherId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
