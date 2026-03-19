using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;

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
        var skip = (query.Page - 1) * query.PageSize;

        IReadOnlyList<DataShareSummaryResponse> response =
            await repository.GetIncomingSharesAsync(query.ResearcherId, skip, query.PageSize, cancellationToken);

        return Result<IReadOnlyList<DataShareSummaryResponse>>.Success(response);
    }
}
