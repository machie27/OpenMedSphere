using System.Security.Claims;

namespace OpenMedSphere.API.Extensions;

/// <summary>
/// Extension methods for extracting identity information from <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Custom claim type for the internal researcher GUID, set by the token service after registration.
    /// </summary>
    public const string ResearcherIdClaimType = "oms:researcher_id";

    /// <summary>
    /// Attempts to extract the internal researcher GUID from the custom <c>oms:researcher_id</c> claim.
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <param name="researcherId">The parsed researcher ID if successful.</param>
    /// <returns>True if a valid GUID was found; otherwise, false.</returns>
    public static bool TryGetResearcherId(this ClaimsPrincipal user, out Guid researcherId)
    {
        string? claimValue = user.FindFirstValue(ResearcherIdClaimType);
        return Guid.TryParse(claimValue, out researcherId);
    }

    /// <summary>
    /// Attempts to extract the external identity from the JWT NameIdentifier claim.
    /// Unlike <see cref="TryGetResearcherId"/>, this returns the raw string value,
    /// which may be a non-GUID format (e.g., <c>auth0|123abc</c> from external IdPs).
    /// </summary>
    /// <param name="user">The claims principal.</param>
    /// <param name="externalId">The external identity string if found.</param>
    /// <returns>True if a non-empty NameIdentifier claim was found; otherwise, false.</returns>
    public static bool TryGetExternalId(this ClaimsPrincipal user, out string externalId)
    {
        string? claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(claimValue))
        {
            externalId = claimValue;
            return true;
        }

        externalId = string.Empty;
        return false;
    }
}
