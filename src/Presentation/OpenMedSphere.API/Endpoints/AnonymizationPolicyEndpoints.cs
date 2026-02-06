using OpenMedSphere.Application.AnonymizationPolicies.Commands.CreatePolicy;
using OpenMedSphere.Application.AnonymizationPolicies.Queries.GetAllPolicies;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Minimal API endpoints for anonymization policies.
/// </summary>
public static class AnonymizationPolicyEndpoints
{
    /// <summary>
    /// Maps anonymization policy endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapAnonymizationPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/anonymization-policies")
            .WithTags("Anonymization Policies");

        group.MapGet("/", GetAllAsync)
            .WithName("GetAllAnonymizationPolicies")
            .Produces<IReadOnlyList<AnonymizationPolicyResponse>>();

        group.MapPost("/", CreateAsync)
            .WithName("CreateAnonymizationPolicy")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> GetAllAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<AnonymizationPolicyResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<AnonymizationPolicyResponse>>(
                new GetAllPoliciesQuery(),
                cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CreateAsync(
        CreateAnonymizationPolicyCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await mediator.SendAsync<Guid>(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/anonymization-policies/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }
}
