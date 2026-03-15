using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

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

        IReadOnlyList<Researcher> researchers = await repository.SearchAsync(
            query.Query, skip, query.PageSize, cancellationToken);

        IReadOnlyList<ResearcherSummaryResponse> response = researchers
            .Select(r => new ResearcherSummaryResponse
            {
                Id = r.Id,
                Name = r.Name,
                Institution = r.Institution
            })
            .ToList();

        return Result<IReadOnlyList<ResearcherSummaryResponse>>.Success(response);
    }
}
