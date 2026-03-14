using System.Security.Claims;

namespace OpenMedSphere.API.Extensions;

/// <summary>
/// Extension methods for extracting identity information from <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Attempts to extract the researcher ID from the JWT NameIdentifier claim.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <param name="researcherId">The parsed researcher ID if successful.</param>
    /// <returns>True if a valid GUID was found; otherwise, false.</returns>
    public static bool TryGetResearcherId(this ClaimsPrincipal user, out Guid researcherId)
    {
        string? claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claimValue, out researcherId);
    }
}
