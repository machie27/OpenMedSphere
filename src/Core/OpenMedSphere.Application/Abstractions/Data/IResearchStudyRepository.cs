using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Repository interface for research studies.
/// </summary>
public interface IResearchStudyRepository : IRepository<ResearchStudy, Guid>
{
    /// <summary>
    /// Gets a research study by its study code.
    /// </summary>
    /// <param name="code">The study code to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The research study, or null if not found.</returns>
    Task<ResearchStudy?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active research studies.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All active research studies.</returns>
    Task<IReadOnlyList<ResearchStudy>> GetActiveStudiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets research studies by research area.
    /// </summary>
    /// <param name="researchArea">The research area to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Research studies matching the research area.</returns>
    Task<IReadOnlyList<ResearchStudy>> GetByResearchAreaAsync(string researchArea, CancellationToken cancellationToken = default);
}
