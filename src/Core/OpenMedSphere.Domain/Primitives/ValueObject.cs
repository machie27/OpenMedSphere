namespace OpenMedSphere.Domain.Primitives;

/// <summary>
/// Base class for all value objects in the domain.
/// Value objects are immutable objects that have no conceptual identity and are defined by their attributes.
/// Note: Consider using C# records for value objects as they provide value-based equality by default.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the components that define equality for this value object.
    /// </summary>
    /// <returns>An enumerable of objects representing the equality components.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other) =>
        other is not null && (ReferenceEquals(this, other) || GetEqualityComponents().SequenceEqual(other.GetEqualityComponents()));

    public override bool Equals(object? obj) =>
        obj is ValueObject valueObject && Equals(valueObject);

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Aggregate(default(int), (hashCode, obj) =>
                HashCode.Combine(hashCode, obj?.GetHashCode() ?? 0));

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !(left == right);
}
