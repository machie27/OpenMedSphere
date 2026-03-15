using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Abstractions.Data;

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
    /// Searches active researchers by name, email, or institution, ordered by most recent first.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="skip">The number of results to skip.</param>
    /// <param name="take">The number of results to take.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Active researchers matching the query.</returns>
    Task<IReadOnlyList<Researcher>> SearchAsync(
        string query, int skip, int take, CancellationToken cancellationToken = default);
}
