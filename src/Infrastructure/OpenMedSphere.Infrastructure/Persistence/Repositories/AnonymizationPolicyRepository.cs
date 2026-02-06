using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for anonymization policies.
/// </summary>
internal sealed class AnonymizationPolicyRepository(ApplicationDbContext dbContext)
    : Repository<AnonymizationPolicy, Guid>(dbContext), IAnonymizationPolicyRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AnonymizationPolicy>> GetActivePoliciesAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(a => a.IsActive)
            .ToListAsync(cancellationToken);
}
