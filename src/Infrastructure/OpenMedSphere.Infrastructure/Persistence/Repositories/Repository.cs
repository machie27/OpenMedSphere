using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Abstractions.Specifications;
using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation backed by EF Core.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TId">The type of entity identifier.</typeparam>
internal abstract class Repository<TEntity, TId>(ApplicationDbContext dbContext)
    : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    protected readonly ApplicationDbContext DbContext = dbContext;

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    /// <inheritdoc />
    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public void Update(TEntity entity) =>
        DbSet.Update(entity);

    /// <inheritdoc />
    public void Remove(TEntity entity) =>
        DbSet.Remove(entity);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator.GetCountQuery(DbSet.AsQueryable(), specification)
            .CountAsync(cancellationToken);
}
