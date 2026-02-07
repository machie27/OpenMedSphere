namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Handles a query that returns a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query.</returns>
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
