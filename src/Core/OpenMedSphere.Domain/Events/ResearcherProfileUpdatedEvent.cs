using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when a researcher updates their profile information.
/// </summary>
public sealed record ResearcherProfileUpdatedEvent(Guid ResearcherId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
