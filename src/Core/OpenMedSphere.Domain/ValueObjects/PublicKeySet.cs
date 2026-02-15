namespace OpenMedSphere.Domain.ValueObjects;

/// <summary>
/// Represents a set of public keys for hybrid quantum-safe cryptography.
/// Keys are stored as Base64 strings for JSON serialization and record equality.
/// </summary>
public sealed record PublicKeySet
{
    /// <summary>
    /// Gets the ML-KEM-768 public key (post-quantum key encapsulation).
    /// </summary>
    public required string MlKemPublicKey { get; init; }

    /// <summary>
    /// Gets the ML-DSA-65 public key (post-quantum digital signature).
    /// </summary>
    public required string MlDsaPublicKey { get; init; }

    /// <summary>
    /// Gets the X25519 public key (classical key exchange).
    /// </summary>
    public required string X25519PublicKey { get; init; }

    /// <summary>
    /// Gets the ECDSA P-256 public key (classical digital signature).
    /// </summary>
    public required string EcdsaPublicKey { get; init; }

    /// <summary>
    /// Gets the key version number (monotonically increasing).
    /// </summary>
    public required int KeyVersion { get; init; }

    /// <summary>
    /// Creates a new public key set.
    /// </summary>
    /// <param name="mlKemPublicKey">The ML-KEM-768 public key (Base64).</param>
    /// <param name="mlDsaPublicKey">The ML-DSA-65 public key (Base64).</param>
    /// <param name="x25519PublicKey">The X25519 public key (Base64).</param>
    /// <param name="ecdsaPublicKey">The ECDSA P-256 public key (Base64).</param>
    /// <param name="keyVersion">The key version number.</param>
    /// <returns>A new public key set.</returns>
    public static PublicKeySet Create(
        string mlKemPublicKey,
        string mlDsaPublicKey,
        string x25519PublicKey,
        string ecdsaPublicKey,
        int keyVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mlKemPublicKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(mlDsaPublicKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(x25519PublicKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(ecdsaPublicKey);
        ArgumentOutOfRangeException.ThrowIfLessThan(keyVersion, 1);

        return new PublicKeySet
        {
            MlKemPublicKey = mlKemPublicKey,
            MlDsaPublicKey = mlDsaPublicKey,
            X25519PublicKey = x25519PublicKey,
            EcdsaPublicKey = ecdsaPublicKey,
            KeyVersion = keyVersion
        };
    }
}
