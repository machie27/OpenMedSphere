using OpenMedSphere.Application.Abstractions.Specifications;
using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Generic repository interface for aggregate roots.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public interface IRepository<TEntity, in TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Finds entities matching a specification.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Entities matching the specification.</returns>
    Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching a specification.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of matching entities.</returns>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
