using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.Abstractions.MedicalTerminology;

/// <summary>
/// Defines a provider for a specific medical coding system.
/// Implement this interface to add support for additional coding systems
/// (e.g., ICD-10, SNOMED CT, MeSH) without modifying existing code.
/// </summary>
public interface IMedicalTerminologyProvider
{
    /// <summary>
    /// Gets the coding system identifier (e.g., "ICD-11", "SNOMED-CT").
    /// </summary>
    string CodingSystem { get; }

    /// <summary>
    /// Searches for medical codes matching the given text.
    /// </summary>
    /// <param name="searchText">The text to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of matching medical codes.</returns>
    Task<IReadOnlyList<MedicalCode>> SearchAsync(string searchText, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a medical code by its code value.
    /// </summary>
    /// <param name="code">The code value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The medical code, or null if not found.</returns>
    Task<MedicalCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a medical code by its entity URI.
    /// </summary>
    /// <param name="entityUri">The entity URI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The medical code, or null if not found.</returns>
    Task<MedicalCode?> GetByEntityUriAsync(string entityUri, CancellationToken cancellationToken);
}
