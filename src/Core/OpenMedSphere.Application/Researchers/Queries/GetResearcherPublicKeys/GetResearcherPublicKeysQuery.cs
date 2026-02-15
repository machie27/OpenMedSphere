using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.GetResearcherPublicKeys;

/// <summary>
/// Query to get a researcher's public keys.
/// </summary>
public sealed record GetResearcherPublicKeysQuery : IQuery<PublicKeySetResponse>
{
    /// <summary>
    /// Gets the researcher ID.
    /// </summary>
    public required Guid Id { get; init; }
}
