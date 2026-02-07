using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.Abstractions.MedicalTerminology;

/// <summary>
/// Composite service that aggregates results from all registered
/// <see cref="IMedicalTerminologyProvider"/> instances.
/// </summary>
public interface IMedicalTerminologyService
{
    /// <summary>
    /// Gets the list of coding systems supported by registered providers.
    /// </summary>
    /// <returns>A list of coding system identifiers.</returns>
    IReadOnlyList<string> GetSupportedCodingSystems();

    /// <summary>
    /// Searches for medical codes matching the given text.
    /// When <paramref name="codingSystem"/> is null, searches all providers and merges results.
    /// </summary>
    /// <param name="searchText">The text to search for.</param>
    /// <param name="codingSystem">Optional coding system to restrict the search to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of matching medical codes.</returns>
    Task<IReadOnlyList<MedicalCode>> SearchAsync(string searchText, string? codingSystem = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a medical code by its code value.
    /// When <paramref name="codingSystem"/> is null, queries all providers and returns the first match.
    /// </summary>
    /// <param name="code">The code value.</param>
    /// <param name="codingSystem">Optional coding system to restrict the lookup to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The medical code, or null if not found.</returns>
    Task<MedicalCode?> GetByCodeAsync(string code, string? codingSystem = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a medical code by its entity URI.
    /// When <paramref name="codingSystem"/> is null, queries all providers and returns the first match.
    /// </summary>
    /// <param name="entityUri">The entity URI.</param>
    /// <param name="codingSystem">Optional coding system to restrict the lookup to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The medical code, or null if not found.</returns>
    Task<MedicalCode?> GetByEntityUriAsync(string entityUri, string? codingSystem = null, CancellationToken cancellationToken = default);
}
