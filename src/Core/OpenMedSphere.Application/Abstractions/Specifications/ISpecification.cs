using System.Linq.Expressions;

namespace OpenMedSphere.Application.Abstractions.Specifications;

/// <summary>
/// Defines a specification for querying entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface ISpecification<TEntity>
{
    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    Expression<Func<TEntity, bool>>? FilterExpression { get; }

    /// <summary>
    /// Gets the order-by expression.
    /// </summary>
    Expression<Func<TEntity, object>>? OrderByExpression { get; }

    /// <summary>
    /// Gets a value indicating whether to order descending.
    /// </summary>
    bool IsDescending { get; }

    /// <summary>
    /// Gets the number of records to take.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets the number of records to skip.
    /// </summary>
    int? Skip { get; }
}
