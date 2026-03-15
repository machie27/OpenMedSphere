using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.DataShares.Queries.GetOutgoingShares;

/// <summary>
/// Handles the <see cref="GetOutgoingSharesQuery"/>.
/// </summary>
internal sealed class GetOutgoingSharesQueryHandler(IDataShareRepository repository)
    : IQueryHandler<GetOutgoingSharesQuery, IReadOnlyList<DataShareSummaryResponse>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DataShareSummaryResponse>>> HandleAsync(
        GetOutgoingSharesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        IReadOnlyList<DataShare> shares =
            await repository.GetOutgoingSharesAsync(query.ResearcherId, (page - 1) * pageSize, pageSize, cancellationToken);

        IReadOnlyList<DataShareSummaryResponse> response = shares
            .Select(s => new DataShareSummaryResponse
            {
                Id = s.Id,
                SenderResearcherId = s.SenderResearcherId,
                RecipientResearcherId = s.RecipientResearcherId,
                PatientDataId = s.PatientDataId,
                Status = s.EffectiveStatus,
                SharedAtUtc = s.SharedAtUtc,
                AccessedAtUtc = s.AccessedAtUtc,
                ExpiresAtUtc = s.ExpiresAtUtc
            })
            .ToList();

        return Result<IReadOnlyList<DataShareSummaryResponse>>.Success(response);
    }
}
