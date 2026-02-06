using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Repository interface for anonymization policies.
/// </summary>
public interface IAnonymizationPolicyRepository : IRepository<AnonymizationPolicy, Guid>
{
    /// <summary>
    /// Gets all active anonymization policies.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All active anonymization policies.</returns>
    Task<IReadOnlyList<AnonymizationPolicy>> GetActivePoliciesAsync(CancellationToken cancellationToken = default);
}
