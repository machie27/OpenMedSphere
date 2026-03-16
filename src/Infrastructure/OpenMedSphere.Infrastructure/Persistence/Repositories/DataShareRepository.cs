using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;

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
            .Select(d => new DataShareSummaryResponse
            {
                Id = d.Id,
                SenderResearcherId = d.SenderResearcherId,
                RecipientResearcherId = d.RecipientResearcherId,
                PatientDataId = d.PatientDataId,
                // EffectiveStatus logic duplicated from DataShare.EffectiveStatus for server-side evaluation.
                // Only Pending shares transition to Expired; Accepted/Revoked shares keep their status.
                Status = (d.ExpiresAtUtc.HasValue && d.ExpiresAtUtc.Value <= DateTime.UtcNow && d.Status == DataShareStatus.Pending)
                    ? DataShareStatus.Expired
                    : d.Status,
                SharedAtUtc = d.SharedAtUtc,
                AccessedAtUtc = d.AccessedAtUtc,
                ExpiresAtUtc = d.ExpiresAtUtc
            })
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
            .Select(d => new DataShareSummaryResponse
            {
                Id = d.Id,
                SenderResearcherId = d.SenderResearcherId,
                RecipientResearcherId = d.RecipientResearcherId,
                PatientDataId = d.PatientDataId,
                // EffectiveStatus logic duplicated from DataShare.EffectiveStatus for server-side evaluation.
                // Only Pending shares transition to Expired; Accepted/Revoked shares keep their status.
                Status = (d.ExpiresAtUtc.HasValue && d.ExpiresAtUtc.Value <= DateTime.UtcNow && d.Status == DataShareStatus.Pending)
                    ? DataShareStatus.Expired
                    : d.Status,
                SharedAtUtc = d.SharedAtUtc,
                AccessedAtUtc = d.AccessedAtUtc,
                ExpiresAtUtc = d.ExpiresAtUtc
            })
            .ToListAsync(cancellationToken);
}
