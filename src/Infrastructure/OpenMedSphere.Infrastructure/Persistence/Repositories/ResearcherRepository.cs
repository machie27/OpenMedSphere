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
    public async Task<IReadOnlyList<Researcher>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        string escapedQuery = EscapeLikePattern(query);

        return await DbSet
            .Where(r => r.IsActive &&
                        (EF.Functions.ILike(r.Name, $"%{escapedQuery}%") ||
                         EF.Functions.ILike(r.Email, $"%{escapedQuery}%") ||
                         EF.Functions.ILike(r.Institution, $"%{escapedQuery}%")))
            .ToListAsync(cancellationToken);
    }

    private static string EscapeLikePattern(string input) =>
        input
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}
