using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.DataShares.Queries;

/// <summary>
/// Full response DTO for a data share, including the encrypted payload.
/// </summary>
public sealed record DataShareResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the sender researcher ID.
    /// </summary>
    public required Guid SenderResearcherId { get; init; }

    /// <summary>
    /// Gets the recipient researcher ID.
    /// </summary>
    public required Guid RecipientResearcherId { get; init; }

    /// <summary>
    /// Gets the patient data ID.
    /// </summary>
    public required Guid PatientDataId { get; init; }

    /// <summary>
    /// Gets the Base64-encoded encrypted payload.
    /// </summary>
    public required string EncryptedPayload { get; init; }

    /// <summary>
    /// Gets the Base64-encoded encapsulated key.
    /// </summary>
    public required string EncapsulatedKey { get; init; }

    /// <summary>
    /// Gets the Base64-encoded signature.
    /// </summary>
    public required string Signature { get; init; }

    /// <summary>
    /// Gets the sender's key version.
    /// </summary>
    public int SenderKeyVersion { get; init; }

    /// <summary>
    /// Gets the recipient's key version.
    /// </summary>
    public int RecipientKeyVersion { get; init; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public DataShareStatus Status { get; init; }

    /// <summary>
    /// Gets the shared date.
    /// </summary>
    public DateTime SharedAtUtc { get; init; }

    /// <summary>
    /// Gets the expiry date.
    /// </summary>
    public DateTime? ExpiresAtUtc { get; init; }
}

/// <summary>
/// Summary response DTO for a data share (metadata only, no encrypted payload).
/// </summary>
public sealed record DataShareSummaryResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the sender researcher ID.
    /// </summary>
    public required Guid SenderResearcherId { get; init; }

    /// <summary>
    /// Gets the recipient researcher ID.
    /// </summary>
    public required Guid RecipientResearcherId { get; init; }

    /// <summary>
    /// Gets the patient data ID.
    /// </summary>
    public required Guid PatientDataId { get; init; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public DataShareStatus Status { get; init; }

    /// <summary>
    /// Gets the shared date.
    /// </summary>
    public DateTime SharedAtUtc { get; init; }

    /// <summary>
    /// Gets the accessed date.
    /// </summary>
    public DateTime? AccessedAtUtc { get; init; }

    /// <summary>
    /// Gets the expiry date.
    /// </summary>
    public DateTime? ExpiresAtUtc { get; init; }
}
