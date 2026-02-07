using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Events;

/// <summary>
/// Domain event raised when a new research study is created.
/// </summary>
public sealed record ResearchStudyCreatedEvent(Guid StudyId, string StudyCode) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
