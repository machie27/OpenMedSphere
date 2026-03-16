using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;

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
        var skip = (query.Page - 1) * query.PageSize;

        IReadOnlyList<DataShareSummaryResponse> response =
            await repository.GetOutgoingSharesAsync(query.ResearcherId, skip, query.PageSize, cancellationToken);

        return Result<IReadOnlyList<DataShareSummaryResponse>>.Success(response);
    }
}
