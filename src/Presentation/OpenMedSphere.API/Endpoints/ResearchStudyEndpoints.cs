using OpenMedSphere.Application.Common;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.ResearchStudies.Commands.CreateResearchStudy;
using OpenMedSphere.Application.ResearchStudies.Queries.SearchResearchStudies;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Minimal API endpoints for research studies.
/// </summary>
public static class ResearchStudyEndpoints
{
    /// <summary>
    /// Maps research study endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapResearchStudyEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/research-studies")
            .WithTags("Research Studies");

        group.MapGet("/search", SearchAsync)
            .WithName("SearchResearchStudies")
            .Produces<PagedResult<ResearchStudyResponse>>();

        group.MapPost("/", CreateAsync)
            .WithName("CreateResearchStudy")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> SearchAsync(
        [AsParameters] SearchResearchStudiesQueryParameters parameters,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        SearchResearchStudiesQuery query = new()
        {
            ResearchArea = parameters.ResearchArea,
            TitleSearch = parameters.TitleSearch,
            ActiveOnly = parameters.ActiveOnly,
            OverlapStart = parameters.OverlapStart,
            OverlapEnd = parameters.OverlapEnd,
            Page = parameters.Page ?? 1,
            PageSize = parameters.PageSize ?? 20
        };

        Result<PagedResult<ResearchStudyResponse>> result =
            await mediator.QueryAsync<PagedResult<ResearchStudyResponse>>(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateAsync(
        CreateResearchStudyCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await mediator.SendAsync<Guid>(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/research-studies/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }
}

/// <summary>
/// Query string parameters for searching research studies.
/// </summary>
public sealed record SearchResearchStudiesQueryParameters
{
    /// <summary>
    /// Gets the research area to search for.
    /// </summary>
    public string? ResearchArea { get; init; }

    /// <summary>
    /// Gets the title text to search for.
    /// </summary>
    public string? TitleSearch { get; init; }

    /// <summary>
    /// Gets whether to filter by active status.
    /// </summary>
    public bool? ActiveOnly { get; init; }

    /// <summary>
    /// Gets the study period overlap start.
    /// </summary>
    public DateTime? OverlapStart { get; init; }

    /// <summary>
    /// Gets the study period overlap end.
    /// </summary>
    public DateTime? OverlapEnd { get; init; }

    /// <summary>
    /// Gets the page number.
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int? PageSize { get; init; }
}
