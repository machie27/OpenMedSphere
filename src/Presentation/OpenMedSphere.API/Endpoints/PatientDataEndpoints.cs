using OpenMedSphere.Application.Common;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.PatientData.Commands.AnonymizePatientData;
using OpenMedSphere.Application.PatientData.Commands.CreatePatientData;
using OpenMedSphere.Application.PatientData.Queries.GetPatientDataById;
using OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Minimal API endpoints for patient data.
/// </summary>
public static class PatientDataEndpoints
{
    /// <summary>
    /// Maps patient data endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapPatientDataEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/patient-data")
            .WithTags("Patient Data");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetPatientDataById")
            .Produces<PatientDataResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/search", SearchAsync)
            .WithName("SearchPatientData")
            .Produces<PagedResult<PatientDataResponse>>();

        group.MapPost("/", CreateAsync)
            .WithName("CreatePatientData")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/anonymize", AnonymizeAsync)
            .WithName("AnonymizePatientData")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<PatientDataResponse> result = await mediator.QueryAsync<PatientDataResponse>(
            new GetPatientDataByIdQuery { Id = id },
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> SearchAsync(
        [AsParameters] SearchPatientDataQueryParameters parameters,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        SearchPatientDataQuery query = new()
        {
            DiagnosisText = parameters.DiagnosisText,
            IcdCode = parameters.IcdCode,
            Region = parameters.Region,
            AnonymizedOnly = parameters.AnonymizedOnly,
            CollectedAfter = parameters.CollectedAfter,
            CollectedBefore = parameters.CollectedBefore,
            Page = parameters.Page ?? 1,
            PageSize = parameters.PageSize ?? 20
        };

        Result<PagedResult<PatientDataResponse>> result =
            await mediator.QueryAsync<PagedResult<PatientDataResponse>>(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateAsync(
        CreatePatientDataCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await mediator.SendAsync<Guid>(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/patient-data/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> AnonymizeAsync(
        Guid id,
        AnonymizePatientDataRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        AnonymizePatientDataCommand command = new()
        {
            PatientDataId = id,
            PolicyId = request.PolicyId
        };

        Result result = await mediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }
}

/// <summary>
/// Query string parameters for searching patient data.
/// </summary>
public sealed record SearchPatientDataQueryParameters
{
    /// <summary>
    /// Gets the diagnosis text to search for.
    /// </summary>
    public string? DiagnosisText { get; init; }

    /// <summary>
    /// Gets the ICD code to filter by.
    /// </summary>
    public string? IcdCode { get; init; }

    /// <summary>
    /// Gets the region to filter by.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Gets whether to filter by anonymized status.
    /// </summary>
    public bool? AnonymizedOnly { get; init; }

    /// <summary>
    /// Gets the minimum collection date.
    /// </summary>
    public DateTime? CollectedAfter { get; init; }

    /// <summary>
    /// Gets the maximum collection date.
    /// </summary>
    public DateTime? CollectedBefore { get; init; }

    /// <summary>
    /// Gets the page number.
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int? PageSize { get; init; }
}

/// <summary>
/// Request body for anonymizing patient data.
/// </summary>
public sealed record AnonymizePatientDataRequest
{
    /// <summary>
    /// Gets the policy ID to apply.
    /// </summary>
    public required Guid PolicyId { get; init; }
}
