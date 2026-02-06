using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OpenMedSphere.API.Endpoints;

/// <summary>
/// Development-only authentication endpoints for generating test tokens.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps development authentication endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/dev-token", GenerateDevToken)
            .WithName("GenerateDevToken")
            .AllowAnonymous()
            .Produces<DevTokenResponse>();

        return app;
    }

    private static IResult GenerateDevToken(IConfiguration configuration)
    {
        string key = configuration["Jwt:Key"] ?? "OpenMedSphere-Development-Key-That-Is-At-Least-32-Bytes!";
        string issuer = configuration["Jwt:Issuer"] ?? "OpenMedSphere-Dev";
        string audience = configuration["Jwt:Audience"] ?? "OpenMedSphere-Dev";

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.CreateVersion7().ToString()),
            new Claim(ClaimTypes.Name, "dev-user"),
            new Claim(ClaimTypes.Role, "Admin")
        ];

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new DevTokenResponse { Token = tokenString, ExpiresAt = token.ValidTo });
    }
}

/// <summary>
/// Response containing a development JWT token.
/// </summary>
public sealed record DevTokenResponse
{
    /// <summary>
    /// Gets the JWT token string.
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Gets the token expiration time.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
