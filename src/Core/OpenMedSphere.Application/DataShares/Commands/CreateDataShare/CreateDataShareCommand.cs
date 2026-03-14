using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Commands.CreateDataShare;

/// <summary>
/// Command to create a new encrypted data share between researchers.
/// </summary>
public sealed record CreateDataShareCommand : ICommand<Guid>
{
    /// <summary>
    /// Gets the sender researcher's ID.
    /// </summary>
    public required Guid SenderResearcherId { get; init; }

    /// <summary>
    /// Gets the recipient researcher's ID.
    /// </summary>
    public required Guid RecipientResearcherId { get; init; }

    /// <summary>
    /// Gets the patient data ID.
    /// </summary>
    public required Guid PatientDataId { get; init; }

    /// <summary>
    /// Gets the Base64-encoded encrypted payload (AES-256-GCM ciphertext).
    /// </summary>
    public required string EncryptedPayload { get; init; }

    /// <summary>
    /// Gets the Base64-encoded hybrid KEM encapsulated key.
    /// </summary>
    public required string EncapsulatedKey { get; init; }

    /// <summary>
    /// Gets the Base64-encoded hybrid signature.
    /// </summary>
    public required string Signature { get; init; }

    /// <summary>
    /// Gets the sender's key version.
    /// </summary>
    public required int SenderKeyVersion { get; init; }

    /// <summary>
    /// Gets the recipient's key version.
    /// </summary>
    public required int RecipientKeyVersion { get; init; }

    /// <summary>
    /// Gets the optional expiry date.
    /// </summary>
    public DateTime? ExpiresAtUtc { get; init; }
}
