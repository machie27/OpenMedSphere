using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Commands.RegisterResearcher;

/// <summary>
/// Command to register a new researcher with cryptographic keys.
/// </summary>
public sealed record RegisterResearcherCommand : ICommand<Guid>
{
    /// <summary>
    /// Gets the researcher's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the researcher's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the researcher's institution.
    /// </summary>
    public required string Institution { get; init; }

    /// <summary>
    /// Gets the ML-KEM-768 public key (Base64).
    /// </summary>
    public required string MlKemPublicKey { get; init; }

    /// <summary>
    /// Gets the ML-DSA-65 public key (Base64).
    /// </summary>
    public required string MlDsaPublicKey { get; init; }

    /// <summary>
    /// Gets the X25519 public key (Base64).
    /// </summary>
    public required string X25519PublicKey { get; init; }

    /// <summary>
    /// Gets the ECDSA P-256 public key (Base64).
    /// </summary>
    public required string EcdsaPublicKey { get; init; }
}
