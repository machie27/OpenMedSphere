namespace OpenMedSphere.Domain.Primitives;

/// <summary>
/// Base class for all entities in the domain.
/// Entities are objects that have a distinct identity that runs through time and different states.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Gets the entity's unique identifier.
    /// </summary>
    public TId Id { get; protected init; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class.
    /// </summary>
    /// <param name="id">The entity's unique identifier.</param>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class.
    /// Required for EF Core.
    /// </summary>
    protected Entity()
    {
    }

    public bool Equals(Entity<TId>? other) =>
        other is not null && (ReferenceEquals(this, other) || EqualityComparer<TId>.Default.Equals(Id, other.Id));

    public override bool Equals(object? obj) =>
        obj is Entity<TId> entity && Equals(entity);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !(left == right);
}
