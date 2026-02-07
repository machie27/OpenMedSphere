using System.Security.Claims;
using Microsoft.Extensions.Logging;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Application.MedicalTerminology.Queries.SearchMedicalCodes;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Minimal API endpoints for medical terminology lookups.
/// </summary>
public static class MedicalTerminologyEndpoints
{
    /// <summary>
    /// Maps medical terminology endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapMedicalTerminologyEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/medical-codes")
            .WithTags("Medical Terminology")
            .RequireAuthorization()
            .RequireRateLimiting("fixed")
            .AddEndpointFilter(async (context, next) =>
            {
                var user = context.HttpContext.User;
                var userId = SanitizeLogValue(user.FindFirstValue(ClaimTypes.NameIdentifier));
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(MedicalTerminologyEndpoints));

                logger.LogInformation(
                    "User {UserId} accessing medical terminology: {Method} {Path}",
                    userId,
                     SanitizeLogValue(context.HttpContext.Request.Method),
                    SanitizeLogValue(context.HttpContext.Request.Path));

                return await next(context);
            });

        group.MapGet("/coding-systems", GetCodingSystems)
            .WithName("GetCodingSystems")
            .Produces<IReadOnlyList<string>>();

        group.MapGet("/search", SearchAsync)
            .WithName("SearchMedicalCodes")
            .Produces<IReadOnlyList<MedicalCodeResponse>>();

        group.MapGet("/{code}", GetByCodeAsync)
            .WithName("GetMedicalCodeByCode")
            .Produces<MedicalCodeResponse>()
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static IResult GetCodingSystems(IMedicalTerminologyService terminologyService)
    {
        IReadOnlyList<string> systems = terminologyService.GetSupportedCodingSystems();
        return Results.Ok(systems);
    }

    private static async Task<IResult> SearchAsync(
        string q,
        string? codingSystem,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        SearchMedicalCodesQuery query = new() { SearchText = q, CodingSystem = codingSystem };

        Result<IReadOnlyList<MedicalCodeResponse>> result =
            await mediator.QueryAsync<IReadOnlyList<MedicalCodeResponse>>(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetByCodeAsync(
        string code,
        string? codingSystem,
        IMedicalTerminologyService terminologyService,
        CancellationToken cancellationToken)
    {
        Domain.ValueObjects.MedicalCode? result =
            await terminologyService.GetByCodeAsync(code, codingSystem, cancellationToken);

        if (result is null)
        {
            return Results.NotFound($"Medical code '{code}' not found.");
        }

        MedicalCodeResponse response = new()
        {
            Code = result.Code,
            DisplayName = result.DisplayName,
            CodingSystem = result.CodingSystem,
            EntityUri = result.EntityUri
        };

        return Results.Ok(response);
    }

    /// <summary>
    /// Strips line endings from a value before logging to prevent log forging (CWE-117).
    /// </summary>
    private static string SanitizeLogValue(string? value) =>
        (value ?? "unknown").ReplaceLineEndings("");
}
