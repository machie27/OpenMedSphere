using System.Linq.Expressions;

namespace OpenMedSphere.Application.Abstractions.Specifications;

/// <summary>
/// Abstract base class for specifications with a builder-style API.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public abstract class Specification<TEntity> : ISpecification<TEntity>
{
    /// <inheritdoc />
    public Expression<Func<TEntity, bool>>? FilterExpression { get; private set; }

    /// <inheritdoc />
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }

    /// <inheritdoc />
    public bool IsDescending { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <summary>
    /// Adds a filter expression to the specification.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    protected void AddFilter(Expression<Func<TEntity, bool>> filter)
    {
        FilterExpression = FilterExpression is null
            ? filter
            : CombineFilters(FilterExpression, filter);
    }

    /// <summary>
    /// Adds an order-by expression to the specification.
    /// </summary>
    /// <param name="orderBy">The order-by expression.</param>
    protected void AddOrderBy(Expression<Func<TEntity, object>> orderBy)
    {
        OrderByExpression = orderBy;
        IsDescending = false;
    }

    /// <summary>
    /// Adds a descending order-by expression to the specification.
    /// </summary>
    /// <param name="orderByDescending">The order-by expression.</param>
    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescending)
    {
        OrderByExpression = orderByDescending;
        IsDescending = true;
    }

    /// <summary>
    /// Applies paging to the specification.
    /// </summary>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    private static Expression<Func<TEntity, bool>> CombineFilters(
        Expression<Func<TEntity, bool>> left,
        Expression<Func<TEntity, bool>> right)
    {
        ParameterExpression parameter = left.Parameters[0];

        ReplaceParameterVisitor visitor = new(right.Parameters[0], parameter);
        Expression rightBody = visitor.Visit(right.Body);

        BinaryExpression combined = Expression.AndAlso(left.Body, rightBody);
        return Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == oldParam ? newParam : base.VisitParameter(node);
    }
}
