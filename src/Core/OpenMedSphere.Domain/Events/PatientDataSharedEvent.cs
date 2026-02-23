using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when patient data is shared between researchers.
/// </summary>
public sealed record PatientDataSharedEvent(
    Guid DataShareId,
    Guid SenderResearcherId,
    Guid RecipientResearcherId,
    Guid PatientDataId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
