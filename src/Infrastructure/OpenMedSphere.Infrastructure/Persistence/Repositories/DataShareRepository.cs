using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for data shares.
/// </summary>
internal sealed class DataShareRepository(ApplicationDbContext dbContext)
    : Repository<DataShare, Guid>(dbContext), IDataShareRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<DataShareSummaryResponse>> GetIncomingSharesAsync(
        Guid recipientResearcherId,
        int skip,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(d => d.RecipientResearcherId == recipientResearcherId)
            .OrderByDescending(d => d.SharedAtUtc)
            .Skip(skip)
            .Take(take)
            .Select(DataShareProjections.ToSummary)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<DataShareSummaryResponse>> GetOutgoingSharesAsync(
        Guid senderResearcherId,
        int skip,
        int take,
        CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(d => d.SenderResearcherId == senderResearcherId)
            .OrderByDescending(d => d.SharedAtUtc)
            .Skip(skip)
            .Take(take)
            .Select(DataShareProjections.ToSummary)
            .ToListAsync(cancellationToken);
}
