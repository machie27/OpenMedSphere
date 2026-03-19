using OpenMedSphere.Application.Researchers.Queries;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Database index names for the Researcher entity, shared between configuration and handlers
/// to prevent silent constraint-detection failures if an index is renamed.
/// </summary>
public static class ResearcherIndexNames
{
    public const string EmailUnique = "IX_Researchers_Email";
    public const string ExternalIdUnique = "IX_Researchers_ExternalId";
}

/// <summary>
/// Repository interface for researchers.
/// </summary>
public interface IResearcherRepository : IRepository<Researcher, Guid>
{
    /// <summary>
    /// Gets a researcher by email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The researcher, or null if not found.</returns>
    Task<Researcher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a researcher by their external identity identifier.
    /// </summary>
    /// <param name="externalId">The external identity identifier to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The researcher, or null if not found.</returns>
    Task<Researcher?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches active researchers by name, email, or institution, ordered by most recent first.
    /// Uses server-side projection to avoid loading public key columns.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="skip">The number of results to skip.</param>
    /// <param name="take">The number of results to take.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Summary DTOs for active researchers matching the query.</returns>
    Task<IReadOnlyList<ResearcherSummaryResponse>> SearchAsync(
        string query, int skip, int take, CancellationToken cancellationToken = default);
}
