namespace OpenMedSphere.Application.Researchers.Queries;

/// <summary>
/// Response DTO for a researcher.
/// </summary>
public sealed record ResearcherResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

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
    /// Gets the current key version.
    /// </summary>
    public int KeyVersion { get; init; }

    /// <summary>
    /// Gets whether the researcher is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }
}

/// <summary>
/// Response DTO for a researcher's public keys.
/// </summary>
public sealed record PublicKeySetResponse
{
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

    /// <summary>
    /// Gets the key version.
    /// </summary>
    public required int KeyVersion { get; init; }
}

/// <summary>
/// Summary response DTO for researcher search results.
/// </summary>
public sealed record ResearcherSummaryResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

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
}
