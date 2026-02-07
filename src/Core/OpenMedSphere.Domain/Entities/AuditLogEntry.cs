using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Entities;

/// <summary>
/// Represents an audit log entry tracking changes to entities.
/// </summary>
public sealed class AuditLogEntry : Entity<Guid>
{
    /// <summary>
    /// Gets the type name of the entity that was changed.
    /// </summary>
    public required string EntityType { get; init; }

    /// <summary>
    /// Gets the identifier of the entity that was changed.
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// Gets the action that was performed (Created, Modified, Deleted).
    /// </summary>
    public required string Action { get; init; }

    /// <summary>
    /// Gets the old values as a JSON string (for Modified and Deleted actions).
    /// </summary>
    public string? OldValues { get; init; }

    /// <summary>
    /// Gets the new values as a JSON string (for Created and Modified actions).
    /// </summary>
    public string? NewValues { get; init; }

    /// <summary>
    /// Gets the user ID who performed the action.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets the date and time when the change occurred.
    /// </summary>
    public DateTime OccurredAtUtc { get; init; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private AuditLogEntry() : base()
    {
    }

    /// <summary>
    /// Creates a new audit log entry.
    /// </summary>
    /// <param name="entityType">The type name of the entity.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="action">The action performed.</param>
    /// <param name="oldValues">The old values as JSON.</param>
    /// <param name="newValues">The new values as JSON.</param>
    /// <param name="userId">The user who performed the action.</param>
    /// <returns>A new audit log entry.</returns>
    public static AuditLogEntry Create(
        string entityType,
        string entityId,
        string action,
        string? oldValues,
        string? newValues,
        string? userId)
    {
        return new AuditLogEntry(Guid.CreateVersion7())
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues,
            NewValues = newValues,
            UserId = userId,
            OccurredAtUtc = DateTime.UtcNow
        };
    }

    private AuditLogEntry(Guid id) : base(id)
    {
    }
}
