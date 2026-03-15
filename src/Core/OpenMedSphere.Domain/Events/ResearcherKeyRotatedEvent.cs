using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when a researcher rotates their public keys.
/// </summary>
public sealed record ResearcherKeyRotatedEvent(
    Guid ResearcherId,
    int OldKeyVersion,
    int NewKeyVersion) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
