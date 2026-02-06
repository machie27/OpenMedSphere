namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Validates an instance of the specified type.
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified instance.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>The validation result.</returns>
    ValidationResult Validate(T instance);
}
