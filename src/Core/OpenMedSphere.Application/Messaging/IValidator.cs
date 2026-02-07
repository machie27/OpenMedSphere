namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Validates an instance of the specified type.
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified instance asynchronously.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
