namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Marker interface for queries that return a <see cref="Result{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<TResponse>
{
}
