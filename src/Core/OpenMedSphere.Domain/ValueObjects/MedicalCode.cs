namespace OpenMedSphere.Domain.ValueObjects;

/// <summary>
/// Represents a structured medical code from a recognized coding system (e.g., ICD-11).
/// </summary>
public sealed record MedicalCode
{
    /// <summary>
    /// Gets the code value (e.g., "BA00", "5A11").
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the display name of the code (e.g., "Essential hypertension").
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets the coding system identifier (e.g., "ICD-11", "ICD-10").
    /// </summary>
    public required string CodingSystem { get; init; }

    /// <summary>
    /// Gets the optional entity URI from the coding system API.
    /// </summary>
    public string? EntityUri { get; init; }

    /// <summary>
    /// Creates a new medical code.
    /// </summary>
    /// <param name="code">The code value.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="codingSystem">The coding system identifier.</param>
    /// <param name="entityUri">The optional entity URI.</param>
    /// <returns>A new medical code.</returns>
    public static MedicalCode Create(string code, string displayName, string codingSystem, string? entityUri = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(codingSystem);

        return new MedicalCode
        {
            Code = code,
            DisplayName = displayName,
            CodingSystem = codingSystem,
            EntityUri = entityUri
        };
    }
}
