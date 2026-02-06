using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for research studies.
/// </summary>
internal sealed class ResearchStudyRepository(ApplicationDbContext dbContext)
    : Repository<ResearchStudy, Guid>(dbContext), IResearchStudyRepository
{
    /// <inheritdoc />
    public async Task<ResearchStudy?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .FirstOrDefaultAsync(r => r.Code.Value == code.ToUpperInvariant(), cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResearchStudy>> GetActiveStudiesAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResearchStudy>> GetByResearchAreaAsync(
        string researchArea,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(r => r.ResearchArea != null &&
                        EF.Functions.ILike(r.ResearchArea, $"%{researchArea}%"))
            .ToListAsync(cancellationToken);
}
