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
        IReadOnlyList<DataShare> shares =
            await repository.GetIncomingSharesAsync(query.ResearcherId, cancellationToken);

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
