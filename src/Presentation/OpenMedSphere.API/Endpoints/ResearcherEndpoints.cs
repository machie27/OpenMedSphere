using System.Security.Claims;
using OpenMedSphere.API.Extensions;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Researchers.Commands.RegisterResearcher;
using OpenMedSphere.Application.Researchers.Commands.UpdateResearcherPublicKeys;
using OpenMedSphere.Application.Researchers.Queries;
using OpenMedSphere.Application.Researchers.Queries.GetResearcherById;
using OpenMedSphere.Application.Researchers.Queries.GetResearcherPublicKeys;
using OpenMedSphere.Application.Researchers.Queries.SearchResearchers;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Minimal API endpoints for researchers.
/// </summary>
public static class ResearcherEndpoints
{
    /// <summary>
    /// Maps researcher endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapResearcherEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/researchers")
            .WithTags("Researchers")
            .RequireAuthorization();

        group.MapPost("/", RegisterAsync)
            .WithName("RegisterResearcher")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .RequireRateLimiting("write");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetResearcherById")
            .Produces<ResearcherResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireRateLimiting("fixed");

        group.MapGet("/{id:guid}/public-keys", GetPublicKeysAsync)
            .WithName("GetResearcherPublicKeys")
            .Produces<PublicKeySetResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireRateLimiting("fixed");

        group.MapGet("/search", SearchAsync)
            .WithName("SearchResearchers")
            .Produces<IReadOnlyList<ResearcherSummaryResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .RequireRateLimiting("fixed");

        group.MapPut("/{id:guid}/public-keys", UpdatePublicKeysAsync)
            .WithName("UpdateResearcherPublicKeys")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting("write");

        return app;
    }

    // Registration is a special case: no researcher ID exists in JWT yet.
    // Authorization is provided by JWT authentication (RequireAuthorization on the group)
    // and email uniqueness (DB unique index prevents duplicate registrations).
    // Future: require an invitation token or admin approval before registration.
    private static async Task<IResult> RegisterAsync(
        RegisterResearcherCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await mediator.SendAsync<Guid>(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Created($"/api/researchers/{result.Value}", result.Value);
        }

        return result.MapError();
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<ResearcherResponse> result = await mediator.QueryAsync<ResearcherResponse>(
            new GetResearcherByIdQuery { Id = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return result.MapError();
    }

    private static async Task<IResult> GetPublicKeysAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<PublicKeySetResponse> result = await mediator.QueryAsync<PublicKeySetResponse>(
            new GetResearcherPublicKeysQuery { Id = id },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return result.MapError();
    }

    private static async Task<IResult> SearchAsync(
        string query,
        IMediator mediator,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<ResearcherSummaryResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<ResearcherSummaryResponse>>(
                new SearchResearchersQuery { Query = query, Page = page, PageSize = pageSize },
                cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return result.MapError();
    }

    private static async Task<IResult> UpdatePublicKeysAsync(
        Guid id,
        UpdateResearcherPublicKeysRequest request,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!user.TryGetResearcherId(out Guid callerId))
        {
            return Results.Unauthorized();
        }

        if (callerId != id)
        {
            return Results.Forbid();
        }

        UpdateResearcherPublicKeysCommand command = new()
        {
            ResearcherId = id,
            MlKemPublicKey = request.MlKemPublicKey,
            MlDsaPublicKey = request.MlDsaPublicKey,
            X25519PublicKey = request.X25519PublicKey,
            EcdsaPublicKey = request.EcdsaPublicKey,
            KeyVersion = request.KeyVersion
        };

        Result result = await mediator.SendAsync(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return result.MapError();
    }

}

/// <summary>
/// Request body for updating researcher public keys.
/// </summary>
public sealed record UpdateResearcherPublicKeysRequest
{
    /// <summary>
    /// Gets the new ML-KEM-768 public key (Base64).
    /// </summary>
    public required string MlKemPublicKey { get; init; }

    /// <summary>
    /// Gets the new ML-DSA-65 public key (Base64).
    /// </summary>
    public required string MlDsaPublicKey { get; init; }

    /// <summary>
    /// Gets the new X25519 public key (Base64).
    /// </summary>
    public required string X25519PublicKey { get; init; }

    /// <summary>
    /// Gets the new ECDSA P-256 public key (Base64).
    /// </summary>
    public required string EcdsaPublicKey { get; init; }

    /// <summary>
    /// Gets the new key version.
    /// </summary>
    public required int KeyVersion { get; init; }
}
