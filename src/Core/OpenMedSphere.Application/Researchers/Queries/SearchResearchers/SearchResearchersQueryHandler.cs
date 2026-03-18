using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.SearchResearchers;

/// <summary>
/// Handles the <see cref="SearchResearchersQuery"/>.
/// </summary>
internal sealed class SearchResearchersQueryHandler(IResearcherRepository repository)
    : IQueryHandler<SearchResearchersQuery, IReadOnlyList<ResearcherSummaryResponse>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ResearcherSummaryResponse>>> HandleAsync(
        SearchResearchersQuery query,
        CancellationToken cancellationToken = default)
    {
        var skip = (query.Page - 1) * query.PageSize;

        IReadOnlyList<ResearcherSummaryResponse> response = await repository.SearchAsync(
            query.Query, skip, query.PageSize, cancellationToken);

        return Result<IReadOnlyList<ResearcherSummaryResponse>>.Success(response);
    }
}
