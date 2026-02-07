using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when patient data is anonymized.
/// </summary>
public sealed record PatientDataAnonymizedEvent(Guid PatientDataId, Guid PolicyId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
