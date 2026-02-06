namespace OpenMedSphere.Application.MedicalTerminology.Queries.SearchMedicalCodes;

/// <summary>
/// Response DTO for medical codes.
/// </summary>
public sealed record MedicalCodeResponse
{
    /// <summary>
    /// Gets the code value.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets the coding system.
    /// </summary>
    public required string CodingSystem { get; init; }

    /// <summary>
    /// Gets the entity URI.
    /// </summary>
    public string? EntityUri { get; init; }
}
