namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Marker interface for commands that return a <see cref="Result"/>.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Marker interface for commands that return a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<TResponse>
{
}
