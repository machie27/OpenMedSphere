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
        IReadOnlyList<DataShare> shares =
            await repository.GetOutgoingSharesAsync(query.ResearcherId, cancellationToken);

        IReadOnlyList<DataShareSummaryResponse> response = shares
            .Select(s => new DataShareSummaryResponse
            {
                Id = s.Id,
                SenderResearcherId = s.SenderResearcherId,
                RecipientResearcherId = s.RecipientResearcherId,
                PatientDataId = s.PatientDataId,
                Status = s.Status,
                SharedAtUtc = s.SharedAtUtc,
                AccessedAtUtc = s.AccessedAtUtc,
                ExpiresAtUtc = s.ExpiresAtUtc
            })
            .ToList();

        return Result<IReadOnlyList<DataShareSummaryResponse>>.Success(response);
    }
}
