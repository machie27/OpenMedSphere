using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.MedicalTerminology.Queries.SearchMedicalCodes;

/// <summary>
/// Query to search medical codes by text, optionally filtered by coding system.
/// </summary>
public sealed record SearchMedicalCodesQuery : IQuery<IReadOnlyList<MedicalCodeResponse>>
{
    /// <summary>
    /// Gets the search text.
    /// </summary>
    public required string SearchText { get; init; }

    /// <summary>
    /// Gets the optional coding system to restrict the search to (e.g., "ICD-11").
    /// When null, all registered providers are searched.
    /// </summary>
    public string? CodingSystem { get; init; }
}
