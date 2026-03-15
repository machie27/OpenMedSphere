using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.DataShares.Queries.GetIncomingShares;

/// <summary>
/// Handles the <see cref="GetIncomingSharesQuery"/>.
/// </summary>
internal sealed class GetIncomingSharesQueryHandler(IDataShareRepository repository)
    : IQueryHandler<GetIncomingSharesQuery, IReadOnlyList<DataShareSummaryResponse>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DataShareSummaryResponse>>> HandleAsync(
        GetIncomingSharesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        IReadOnlyList<DataShare> shares =
            await repository.GetIncomingSharesAsync(query.ResearcherId, (page - 1) * pageSize, pageSize, cancellationToken);

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
