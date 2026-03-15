using System.Security.Claims;
using OpenMedSphere.API.Extensions;
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
            .Produces(StatusCodes.Status422UnprocessableEntity)
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
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting("write");

        group.MapDelete("/{id:guid}", RevokeAsync)
            .WithName("RevokeDataShare")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting("write");

        return app;
    }

    private static async Task<IResult> CreateAsync(
        CreateDataShareRequest request,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetResearcherId(out Guid researcherId))
        {
            return Results.Unauthorized();
        }

        CreateDataShareCommand command = new()
        {
            SenderResearcherId = researcherId,
            RecipientResearcherId = request.RecipientResearcherId,
            PatientDataId = request.PatientDataId,
            EncryptedPayload = request.EncryptedPayload,
            EncapsulatedKey = request.EncapsulatedKey,
            Signature = request.Signature,
            SenderKeyVersion = request.SenderKeyVersion,
            RecipientKeyVersion = request.RecipientKeyVersion,
            ExpiresAtUtc = request.ExpiresAtUtc
        };

        Result<Guid> result = await mediator.SendAsync<Guid>(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Created($"/api/data-shares/{result.Value}", result.Value);
        }

        return MapError(result);
    }

    private static async Task<IResult> GetIncomingAsync(
        ClaimsPrincipal user,
        IMediator mediator,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!user.TryGetResearcherId(out Guid researcherId))
        {
            return Results.Unauthorized();
        }

        Result<IReadOnlyList<DataShareSummaryResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<DataShareSummaryResponse>>(
                new GetIncomingSharesQuery { ResearcherId = researcherId, Page = page, PageSize = pageSize },
                cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return MapError(result);
    }

    private static async Task<IResult> GetOutgoingAsync(
        ClaimsPrincipal user,
        IMediator mediator,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!user.TryGetResearcherId(out Guid researcherId))
        {
            return Results.Unauthorized();
        }

        Result<IReadOnlyList<DataShareSummaryResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<DataShareSummaryResponse>>(
                new GetOutgoingSharesQuery { ResearcherId = researcherId, Page = page, PageSize = pageSize },
                cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return MapError(result);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetResearcherId(out Guid researcherId))
        {
            return Results.Unauthorized();
        }

        Result<DataShareResponse> result = await mediator.QueryAsync<DataShareResponse>(
            new GetDataShareByIdQuery { Id = id, ResearcherId = researcherId },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return MapError(result);
    }

    private static async Task<IResult> AcceptAsync(
        Guid id,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetResearcherId(out Guid researcherId))
        {
            return Results.Unauthorized();
        }

        AcceptDataShareCommand command = new()
        {
            DataShareId = id,
            ResearcherId = researcherId
        };

        Result result = await mediator.SendAsync(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MapError(result);
    }

    private static async Task<IResult> RevokeAsync(
        Guid id,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetResearcherId(out Guid researcherId))
        {
            return Results.Unauthorized();
        }

        RevokeDataShareCommand command = new()
        {
            DataShareId = id,
            ResearcherId = researcherId
        };

        Result result = await mediator.SendAsync(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MapError(result);
    }

    private static IResult MapError(Result result) =>
        result.ErrorCode switch
        {
            ErrorCode.NotFound => Results.NotFound(result.Error),
            ErrorCode.Conflict => Results.Conflict(result.Error),
            ErrorCode.InvalidOperation => Results.UnprocessableEntity(result.Error),
            _ => Results.BadRequest(result.Error)
        };
}

/// <summary>
/// Request body for creating a data share. The sender ID is extracted from JWT claims.
/// </summary>
public sealed record CreateDataShareRequest
{
    /// <summary>
    /// Gets the recipient researcher's ID.
    /// </summary>
    public required Guid RecipientResearcherId { get; init; }

    /// <summary>
    /// Gets the patient data ID.
    /// </summary>
    public required Guid PatientDataId { get; init; }

    /// <summary>
    /// Gets the Base64-encoded encrypted payload (AES-256-GCM ciphertext).
    /// </summary>
    public required string EncryptedPayload { get; init; }

    /// <summary>
    /// Gets the Base64-encoded hybrid KEM encapsulated key.
    /// </summary>
    public required string EncapsulatedKey { get; init; }

    /// <summary>
    /// Gets the Base64-encoded hybrid signature.
    /// </summary>
    public required string Signature { get; init; }

    /// <summary>
    /// Gets the sender's key version.
    /// </summary>
    public required int SenderKeyVersion { get; init; }

    /// <summary>
    /// Gets the recipient's key version.
    /// </summary>
    public required int RecipientKeyVersion { get; init; }

    /// <summary>
    /// Gets the optional expiry date.
    /// </summary>
    public DateTime? ExpiresAtUtc { get; init; }
}
