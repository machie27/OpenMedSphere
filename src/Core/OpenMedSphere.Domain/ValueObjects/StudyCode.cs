namespace OpenMedSphere.Domain.ValueObjects;

/// <summary>
/// Represents a unique code for a research study.
/// </summary>
public sealed record StudyCode
{
    private const int MaxLength = 50;

    /// <summary>
    /// Gets the study code value.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Creates a new study code.
    /// </summary>
    /// <param name="value">The study code value.</param>
    /// <returns>A new study code if validation succeeds.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is invalid.</exception>
    public static StudyCode Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > MaxLength)
        {
            throw new ArgumentException($"Study code cannot exceed {MaxLength} characters.", nameof(value));
        }

        return new StudyCode { Value = value.ToUpperInvariant() };
    }
}
