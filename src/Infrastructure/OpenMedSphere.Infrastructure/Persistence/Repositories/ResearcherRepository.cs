using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for researchers.
/// </summary>
internal sealed class ResearcherRepository(ApplicationDbContext dbContext)
    : Repository<Researcher, Guid>(dbContext), IResearcherRepository
{
    /// <inheritdoc />
    public async Task<Researcher?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(r => r.Email == email, cancellationToken);

    /// <inheritdoc />
    // TODO: Add pg_trgm GIN index on name/email/institution for efficient substring search.
    // Current LOWER(col) LIKE '%x%' cannot use B-tree indexes and scans the full table.
    public async Task<IReadOnlyList<Researcher>> SearchAsync(
        string query,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var queryLower = query.ToLower();

        return await DbSet
            .Where(r => r.IsActive &&
                        (r.Name.ToLower().Contains(queryLower) ||
                         r.Email.ToLower().Contains(queryLower) ||
                         r.Institution.ToLower().Contains(queryLower)))
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
