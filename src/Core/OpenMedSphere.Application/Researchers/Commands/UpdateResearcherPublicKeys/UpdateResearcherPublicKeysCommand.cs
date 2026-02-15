using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Commands.UpdateResearcherPublicKeys;

/// <summary>
/// Command to rotate a researcher's public keys.
/// </summary>
public sealed record UpdateResearcherPublicKeysCommand : ICommand
{
    /// <summary>
    /// Gets the researcher's ID.
    /// </summary>
    public required Guid ResearcherId { get; init; }

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
    /// Gets the new key version (must be greater than current version).
    /// </summary>
    public required int KeyVersion { get; init; }
}
