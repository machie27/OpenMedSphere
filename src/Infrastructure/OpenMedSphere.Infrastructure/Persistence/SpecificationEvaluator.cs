using OpenMedSphere.Application.Abstractions.Specifications;

namespace OpenMedSphere.Infrastructure.Persistence;

/// <summary>
/// Evaluates specifications and applies them to IQueryable sources.
/// </summary>
internal static class SpecificationEvaluator
{
    /// <summary>
    /// Applies a specification to an IQueryable source.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="source">The queryable source.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>The filtered and ordered queryable.</returns>
    public static IQueryable<TEntity> GetQuery<TEntity>(
        IQueryable<TEntity> source,
        ISpecification<TEntity> specification)
        where TEntity : class
    {
        IQueryable<TEntity> query = source;

        if (specification.FilterExpression is not null)
        {
            query = query.Where(specification.FilterExpression);
        }

        if (specification.OrderByExpression is not null)
        {
            query = specification.IsDescending
                ? query.OrderByDescending(specification.OrderByExpression)
                : query.OrderBy(specification.OrderByExpression);
        }

        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }

    /// <summary>
    /// Gets a filtered-only query (no ordering/paging) for counting.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="source">The queryable source.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<TEntity> GetCountQuery<TEntity>(
        IQueryable<TEntity> source,
        ISpecification<TEntity> specification)
        where TEntity : class
    {
        IQueryable<TEntity> query = source;

        if (specification.FilterExpression is not null)
        {
            query = query.Where(specification.FilterExpression);
        }

        return query;
    }
}
