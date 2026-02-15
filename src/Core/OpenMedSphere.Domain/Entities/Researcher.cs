using OpenMedSphere.Domain.Primitives;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Domain.Entities;

/// <summary>
/// Represents a researcher who can share and receive encrypted patient data.
/// This is an aggregate root that manages researcher identity and cryptographic keys.
/// </summary>
public sealed class Researcher : AggregateRoot<Guid>
{
    /// <summary>
    /// Gets the researcher's name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets the researcher's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets the researcher's institution.
    /// </summary>
    public required string Institution { get; set; }

    /// <summary>
    /// Gets the researcher's public key set for hybrid quantum-safe encryption.
    /// </summary>
    public required PublicKeySet PublicKeys { get; set; }

    /// <summary>
    /// Gets a value indicating whether the researcher account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the date and time when the researcher was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the researcher was last updated.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private Researcher() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Researcher"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the researcher.</param>
    private Researcher(Guid id) : base(id)
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new researcher.
    /// </summary>
    /// <param name="name">The researcher's name.</param>
    /// <param name="email">The researcher's email address.</param>
    /// <param name="institution">The researcher's institution.</param>
    /// <param name="publicKeys">The researcher's public key set.</param>
    /// <returns>A new researcher.</returns>
    public static Researcher Create(string name, string email, string institution, PublicKeySet publicKeys)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(institution);
        ArgumentNullException.ThrowIfNull(publicKeys);

        var researcher = new Researcher(Guid.CreateVersion7())
        {
            Name = name,
            Email = email,
            Institution = institution,
            PublicKeys = publicKeys
        };

        return researcher;
    }

    /// <summary>
    /// Rotates the researcher's public keys to a new version.
    /// </summary>
    /// <param name="newPublicKeys">The new public key set.</param>
    public void RotateKeys(PublicKeySet newPublicKeys)
    {
        ArgumentNullException.ThrowIfNull(newPublicKeys);

        if (newPublicKeys.KeyVersion <= PublicKeys.KeyVersion)
        {
            throw new ArgumentException(
                $"New key version ({newPublicKeys.KeyVersion}) must be greater than current version ({PublicKeys.KeyVersion}).",
                nameof(newPublicKeys));
        }

        PublicKeys = newPublicKeys;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the researcher's profile information.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="email">The new email address.</param>
    /// <param name="institution">The new institution.</param>
    public void UpdateProfile(string name, string email, string institution)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(institution);

        Name = name;
        Email = email;
        Institution = institution;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the researcher account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the researcher account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
