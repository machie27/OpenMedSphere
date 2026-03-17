using System.Linq.Expressions;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.DataShares.Queries;

/// <summary>
/// Shared EF Core-compatible projection expressions for <see cref="DataShare"/> queries.
/// Centralizes the EffectiveStatus computation to avoid logic duplication across query methods.
/// </summary>
/// <remarks>
/// The EffectiveStatus logic must stay in sync with <see cref="DataShare.EffectiveStatus"/>.
/// This class provides the expression tree form for server-side evaluation in EF Core queries,
/// while the domain property provides the in-memory form for loaded entities.
/// Uses <c>==</c> instead of <c>is</c> because EF Core expression trees don't support pattern matching.
/// </remarks>
public static class DataShareProjections
{
    /// <summary>
    /// Projects a <see cref="DataShare"/> into a <see cref="DataShareSummaryResponse"/> with
    /// EffectiveStatus computed server-side. Only Pending shares transition to Expired;
    /// Accepted/Revoked shares keep their persisted status.
    /// </summary>
    public static readonly Expression<Func<DataShare, DataShareSummaryResponse>> ToSummary =
        d => new DataShareSummaryResponse
        {
            Id = d.Id,
            SenderResearcherId = d.SenderResearcherId,
            RecipientResearcherId = d.RecipientResearcherId,
            PatientDataId = d.PatientDataId,
            Status = (d.ExpiresAtUtc.HasValue && d.ExpiresAtUtc.Value <= DateTime.UtcNow && d.Status == DataShareStatus.Pending)
                ? DataShareStatus.Expired
                : d.Status,
            SharedAtUtc = d.SharedAtUtc,
            AccessedAtUtc = d.AccessedAtUtc,
            ExpiresAtUtc = d.ExpiresAtUtc
        };
}
