namespace OpenMedSphere.Domain.Primitives;

/// <summary>
/// Base class for aggregate roots in the domain.
/// An aggregate root is an entity that serves as the entry point to an aggregate,
/// ensuring consistency boundaries and managing domain events.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the collection of domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class.
    /// </summary>
    /// <param name="id">The aggregate root's unique identifier.</param>
    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class.
    /// Required for EF Core.
    /// </summary>
    protected AggregateRoot() : base()
    {
    }

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
