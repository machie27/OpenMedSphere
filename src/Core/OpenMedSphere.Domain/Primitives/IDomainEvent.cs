namespace OpenMedSphere.Domain.Primitives;

/// <summary>
/// Represents a domain event that occurred in the system.
/// Domain events are immutable objects that represent something that happened in the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredOnUtc { get; }
}
