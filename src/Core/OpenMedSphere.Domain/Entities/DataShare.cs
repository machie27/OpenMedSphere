using OpenMedSphere.Domain.Enums;
using OpenMedSphere.Domain.Events;
using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Entities;

/// <summary>
/// Represents an encrypted data share between researchers.
/// The server stores encrypted blobs opaquely — it never sees plaintext patient data.
/// This is an aggregate root for the data sharing bounded context.
/// </summary>
public sealed class DataShare : AggregateRoot<Guid>
{
    /// <summary>
    /// Gets the ID of the researcher who sent the share.
    /// </summary>
    public required Guid SenderResearcherId { get; init; }

    /// <summary>
    /// Gets the ID of the researcher who receives the share.
    /// </summary>
    public required Guid RecipientResearcherId { get; init; }

    /// <summary>
    /// Gets the ID of the patient data being shared.
    /// </summary>
    public required Guid PatientDataId { get; init; }

    /// <summary>
    /// Gets the Base64-encoded AES-256-GCM ciphertext of the patient data.
    /// </summary>
    public required string EncryptedPayload { get; init; }

    /// <summary>
    /// Gets the Base64-encoded hybrid KEM encapsulated key.
    /// </summary>
    public required string EncapsulatedKey { get; init; }

    /// <summary>
    /// Gets the Base64-encoded hybrid signature (ECDSA + ML-DSA-65).
    /// </summary>
    public required string Signature { get; init; }

    /// <summary>
    /// Gets the sender's key version used for this share.
    /// </summary>
    public required int SenderKeyVersion { get; init; }

    /// <summary>
    /// Gets the recipient's key version used for this share.
    /// </summary>
    public required int RecipientKeyVersion { get; init; }

    /// <summary>
    /// Gets the current status of the data share.
    /// </summary>
    public DataShareStatus Status { get; set; } = DataShareStatus.Pending;

    /// <summary>
    /// Gets the date and time when the data was shared.
    /// </summary>
    public DateTime SharedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the share was accessed by the recipient.
    /// </summary>
    public DateTime? AccessedAtUtc { get; set; }

    /// <summary>
    /// Gets the date and time when the share expires.
    /// </summary>
    public DateTime? ExpiresAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the share was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the share was last updated.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private DataShare() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataShare"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the data share.</param>
    private DataShare(Guid id) : base(id)
    {
        CreatedAtUtc = DateTime.UtcNow;
        SharedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new encrypted data share.
    /// </summary>
    /// <param name="senderResearcherId">The sender researcher's ID.</param>
    /// <param name="recipientResearcherId">The recipient researcher's ID.</param>
    /// <param name="patientDataId">The patient data ID.</param>
    /// <param name="encryptedPayload">The Base64-encoded encrypted payload.</param>
    /// <param name="encapsulatedKey">The Base64-encoded encapsulated key.</param>
    /// <param name="signature">The Base64-encoded signature.</param>
    /// <param name="senderKeyVersion">The sender's key version.</param>
    /// <param name="recipientKeyVersion">The recipient's key version.</param>
    /// <param name="expiresAtUtc">The optional expiry date.</param>
    /// <returns>A new data share.</returns>
    public static DataShare Create(
        Guid senderResearcherId,
        Guid recipientResearcherId,
        Guid patientDataId,
        string encryptedPayload,
        string encapsulatedKey,
        string signature,
        int senderKeyVersion,
        int recipientKeyVersion,
        DateTime? expiresAtUtc = null)
    {
        if (senderResearcherId == recipientResearcherId)
        {
            throw new ArgumentException("Sender and recipient must be different researchers.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedPayload);
        ArgumentException.ThrowIfNullOrWhiteSpace(encapsulatedKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(signature);
        ArgumentOutOfRangeException.ThrowIfLessThan(senderKeyVersion, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(recipientKeyVersion, 1);

        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiry date must be in the future.", nameof(expiresAtUtc));
        }

        var dataShare = new DataShare(Guid.CreateVersion7())
        {
            SenderResearcherId = senderResearcherId,
            RecipientResearcherId = recipientResearcherId,
            PatientDataId = patientDataId,
            EncryptedPayload = encryptedPayload,
            EncapsulatedKey = encapsulatedKey,
            Signature = signature,
            SenderKeyVersion = senderKeyVersion,
            RecipientKeyVersion = recipientKeyVersion,
            ExpiresAtUtc = expiresAtUtc
        };

        dataShare.RaiseDomainEvent(new PatientDataSharedEvent(
            dataShare.Id, senderResearcherId, recipientResearcherId, patientDataId));

        return dataShare;
    }

    /// <summary>
    /// Accepts the data share, marking it as accessed by the recipient.
    /// </summary>
    public void Accept()
    {
        if (Status is not DataShareStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot accept a data share with status '{Status}'.");
        }

        if (IsExpired())
        {
            throw new InvalidOperationException("Cannot accept an expired data share.");
        }

        Status = DataShareStatus.Accepted;
        AccessedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DataShareAccessedEvent(Id, RecipientResearcherId));
    }

    /// <summary>
    /// Revokes the data share.
    /// </summary>
    public void Revoke()
    {
        if (Status is DataShareStatus.Revoked)
        {
            throw new InvalidOperationException("Data share is already revoked.");
        }

        Status = DataShareStatus.Revoked;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DataShareRevokedEvent(Id, SenderResearcherId));
    }

    /// <summary>
    /// Determines whether the data share has expired.
    /// </summary>
    /// <returns>True if the share has expired; otherwise, false.</returns>
    public bool IsExpired() =>
        ExpiresAtUtc.HasValue && ExpiresAtUtc.Value <= DateTime.UtcNow;
}
