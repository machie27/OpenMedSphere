using OpenMedSphere.Domain.Events;
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
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the researcher's email address.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Gets the researcher's institution.
    /// </summary>
    public string Institution { get; private set; } = null!;

    /// <summary>
    /// Gets the researcher's public key set for hybrid quantum-safe encryption.
    /// </summary>
    public PublicKeySet PublicKeys { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the researcher account is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Gets the date and time when the researcher was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the researcher was last updated.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; private set; }

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

        researcher.RaiseDomainEvent(new ResearcherCreatedEvent(researcher.Id));

        return researcher;
    }

    /// <summary>
    /// Rotates the researcher's public keys to a new version.
    /// The new key set must have a version number strictly greater than the current version
    /// to enforce monotonic key versioning.
    /// </summary>
    /// <param name="newPublicKeys">The new public key set.</param>
    /// <exception cref="InvalidOperationException">Thrown when the researcher account is inactive.</exception>
    /// <exception cref="ArgumentException">Thrown when the new key version is not greater than the current version.</exception>
    public void RotateKeys(PublicKeySet newPublicKeys)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot rotate keys for an inactive researcher.");
        }

        ArgumentNullException.ThrowIfNull(newPublicKeys);

        if (newPublicKeys.KeyVersion <= PublicKeys.KeyVersion)
        {
            throw new ArgumentException(
                "New key version must be greater than the current version.",
                nameof(newPublicKeys));
        }

        var oldKeyVersion = PublicKeys.KeyVersion;
        PublicKeys = newPublicKeys;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ResearcherKeyRotatedEvent(Id, oldKeyVersion, newPublicKeys.KeyVersion));
    }

    /// <summary>
    /// Updates the researcher's profile information.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="email">The new email address.</param>
    /// <param name="institution">The new institution.</param>
    /// <exception cref="InvalidOperationException">Thrown when the researcher account is inactive.</exception>
    public void UpdateProfile(string name, string email, string institution)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot update profile for an inactive researcher.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(institution);

        Name = name;
        Email = email;
        Institution = institution;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ResearcherProfileUpdatedEvent(Id));
    }

    /// <summary>
    /// Deactivates the researcher account. No-op if already inactive.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ResearcherDeactivatedEvent(Id));
    }

    /// <summary>
    /// Activates the researcher account. No-op if already active.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new ResearcherActivatedEvent(Id));
    }
}
