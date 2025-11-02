namespace OpenMedSphere.Domain.ValueObjects;

/// <summary>
/// Represents a unique identifier for a patient.
/// This identifier should be anonymized and not contain any personally identifiable information.
/// </summary>
public sealed record PatientIdentifier
{
    /// <summary>
    /// Gets the unique identifier value.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Creates a new patient identifier.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    /// <returns>A new patient identifier if validation succeeds.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is null, empty, or whitespace.</exception>
    public static PatientIdentifier Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return new PatientIdentifier { Value = value };
    }

    /// <summary>
    /// Generates a new random patient identifier using a GUID.
    /// </summary>
    /// <returns>A new patient identifier.</returns>
    public static PatientIdentifier Generate() =>
        new() { Value = Guid.NewGuid().ToString() };
}
