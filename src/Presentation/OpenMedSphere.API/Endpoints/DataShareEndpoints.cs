using OpenMedSphere.Application.DataShares.Commands.AcceptDataShare;
using OpenMedSphere.Application.DataShares.Commands.CreateDataShare;
using OpenMedSphere.Application.DataShares.Commands.RevokeDataShare;
using OpenMedSphere.Application.DataShares.Queries;
using OpenMedSphere.Application.DataShares.Queries.GetDataShareById;
using OpenMedSphere.Application.DataShares.Queries.GetIncomingShares;
using OpenMedSphere.Application.DataShares.Queries.GetOutgoingShares;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Minimal API endpoints for encrypted data shares.
/// </summary>
public static class DataShareEndpoints
{
    /// <summary>
    /// Maps data share endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapDataShareEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/data-shares")
            .WithTags("Data Shares")
            .RequireAuthorization();

        group.MapPost("/", CreateAsync)
            .WithName("CreateDataShare")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireRateLimiting("write");

        group.MapGet("/incoming", GetIncomingAsync)
            .WithName("GetIncomingDataShares")
            .Produces<IReadOnlyList<DataShareSummaryResponse>>()
            .RequireRateLimiting("fixed");

        group.MapGet("/outgoing", GetOutgoingAsync)
            .WithName("GetOutgoingDataShares")
            .Produces<IReadOnlyList<DataShareSummaryResponse>>()
            .RequireRateLimiting("fixed");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetDataShareById")
            .Produces<DataShareResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireRateLimiting("fixed");

        group.MapPut("/{id:guid}/accept", AcceptAsync)
            .WithName("AcceptDataShare")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireRateLimiting("write");

        group.MapDelete("/{id:guid}", RevokeAsync)
            .WithName("RevokeDataShare")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireRateLimiting("write");

        return app;
    }

    private static async Task<IResult> CreateAsync(
        CreateDataShareCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await mediator.SendAsync<Guid>(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/data-shares/{result.Value}", result.Value)
            : result.ErrorCode == ErrorCode.NotFound
                ? Results.NotFound(result.Error)
                : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetIncomingAsync(
        Guid researcherId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        // TODO: Link researcherId to JWT claims when auth is fully integrated
        Result<IReadOnlyList<DataShareSummaryResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<DataShareSummaryResponse>>(
                new GetIncomingSharesQuery { ResearcherId = researcherId },
                cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetOutgoingAsync(
        Guid researcherId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        // TODO: Link researcherId to JWT claims when auth is fully integrated
        Result<IReadOnlyList<DataShareSummaryResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<DataShareSummaryResponse>>(
                new GetOutgoingSharesQuery { ResearcherId = researcherId },
                cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        Guid researcherId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        // TODO: Link researcherId to JWT claims when auth is fully integrated
        Result<DataShareResponse> result = await mediator.QueryAsync<DataShareResponse>(
            new GetDataShareByIdQuery { Id = id, ResearcherId = researcherId },
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ErrorCode == ErrorCode.NotFound
                ? Results.NotFound(result.Error)
                : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> AcceptAsync(
        Guid id,
        AcceptDataShareRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        // TODO: Link researcherId to JWT claims when auth is fully integrated
        AcceptDataShareCommand command = new()
        {
            DataShareId = id,
            ResearcherId = request.ResearcherId
        };

        Result result = await mediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.ErrorCode == ErrorCode.NotFound
                ? Results.NotFound(result.Error)
                : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> RevokeAsync(
        Guid id,
        Guid researcherId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        // TODO: Link researcherId to JWT claims when auth is fully integrated
        RevokeDataShareCommand command = new()
        {
            DataShareId = id,
            ResearcherId = researcherId
        };

        Result result = await mediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.ErrorCode == ErrorCode.NotFound
                ? Results.NotFound(result.Error)
                : Results.BadRequest(result.Error);
    }
}

/// <summary>
/// Request body for accepting a data share.
/// </summary>
public sealed record AcceptDataShareRequest
{
    /// <summary>
    /// Gets the researcher ID of the recipient.
    /// </summary>
    public required Guid ResearcherId { get; init; }
}
