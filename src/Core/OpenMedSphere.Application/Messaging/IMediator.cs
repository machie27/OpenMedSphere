namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Mediator for dispatching commands and queries to their handlers.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a command that returns a <see cref="Result"/>.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command.</returns>
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command that returns a <see cref="Result{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command.</returns>
    Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a query that returns a <see cref="Result{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the query.</returns>
    Task<Result<TResponse>> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
