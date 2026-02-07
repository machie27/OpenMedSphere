using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when a new patient data record is created.
/// </summary>
public sealed record PatientDataCreatedEvent(Guid PatientDataId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
