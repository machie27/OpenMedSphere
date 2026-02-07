namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Handles a command that returns a <see cref="Result"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command.</returns>
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles a command that returns a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Handles the specified command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command.</returns>
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
