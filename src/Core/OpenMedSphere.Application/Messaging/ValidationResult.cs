namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public sealed class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation passed.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public List<ValidationError> Errors { get; init; } = [];

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A successful validation result.</returns>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Creates a validation result with the specified errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A validation result with errors.</returns>
    public static ValidationResult WithErrors(params ValidationError[] errors) => new() { Errors = [.. errors] };
}

/// <summary>
/// Represents a single validation error.
/// </summary>
/// <param name="PropertyName">The name of the property that failed validation.</param>
/// <param name="ErrorMessage">The error message.</param>
public sealed record ValidationError(string PropertyName, string ErrorMessage);
