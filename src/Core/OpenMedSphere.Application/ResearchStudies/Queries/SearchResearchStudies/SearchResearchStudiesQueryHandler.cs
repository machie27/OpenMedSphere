using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Common;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Specifications;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.ResearchStudies.Queries.SearchResearchStudies;

/// <summary>
/// Handles the <see cref="SearchResearchStudiesQuery"/>.
/// </summary>
internal sealed class SearchResearchStudiesQueryHandler(IResearchStudyRepository repository)
    : IQueryHandler<SearchResearchStudiesQuery, PagedResult<ResearchStudyResponse>>
{
    /// <inheritdoc />
    public async Task<Result<PagedResult<ResearchStudyResponse>>> HandleAsync(
        SearchResearchStudiesQuery query,
        CancellationToken cancellationToken = default)
    {
        ResearchStudySearchSpecification specification = new(
            researchArea: query.ResearchArea,
            titleSearch: query.TitleSearch,
            activeOnly: query.ActiveOnly,
            overlapStart: query.OverlapStart,
            overlapEnd: query.OverlapEnd,
            page: query.Page,
            pageSize: query.PageSize);

        IReadOnlyList<ResearchStudy> items =
            await repository.FindAsync(specification, cancellationToken);

        int totalCount = await repository.CountAsync(specification, cancellationToken);

        IReadOnlyList<ResearchStudyResponse> responses = items
            .Select(r => new ResearchStudyResponse
            {
                Id = r.Id,
                StudyCode = r.Code.Value,
                Title = r.Title,
                Description = r.Description,
                PrincipalInvestigator = r.PrincipalInvestigator,
                Institution = r.Institution,
                ResearchArea = r.ResearchArea,
                StudyPeriodStart = r.StudyPeriod.Start,
                StudyPeriodEnd = r.StudyPeriod.End,
                IsActive = r.IsActive,
                ParticipantCount = r.PatientDataIds.Count,
                MaxParticipants = r.MaxParticipants,
                CreatedAtUtc = r.CreatedAtUtc
            })
            .ToList()
            .AsReadOnly();

        PagedResult<ResearchStudyResponse> result = new()
        {
            Items = responses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        return Result<PagedResult<ResearchStudyResponse>>.Success(result);
    }
}
