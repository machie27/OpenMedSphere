using System.Buffers.Text;

namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Shared validation constants and helpers used across validators.
/// </summary>
internal static class ValidationConstants
{
    public const int MaxGenderLength = 50;
    public const int MaxRegionLength = 200;
    public const int MaxDiagnosisLength = 500;
    public const int MaxIcdCodeLength = 50;
    public const int MaxNotesLength = 10000;
    public const int MaxSecondaryDiagnoses = 50;
    public const int MaxMedications = 100;
    public const int MinYearOfBirth = 1900;
    public const int MaxStudyCodeLength = 50;
    public const int MaxTitleLength = 500;
    public const int MaxInvestigatorLength = 200;
    public const int MaxInstitutionLength = 300;
    public const int MaxDescriptionLength = 5000;
    public const int MaxResearchAreaLength = 200;
    public const int MaxSearchTextLength = 200;
    public const int MinPage = 1;
    public const int MaxPageSize = 100;

    public const int MaxNameLength = 200;
    /// <summary>
    /// RFC 5321 maximum email address length (64 local + 1 '@' + 255 domain).
    /// </summary>
    public const int MaxEmailLength = 320;
    /// <summary>
    /// Max Base64-encoded public key size. ML-DSA-65 is the largest at ~2,604 chars;
    /// 4,000 provides comfortable headroom for all key types.
    /// </summary>
    public const int MaxBase64KeyLength = 4000;
    /// <summary>
    /// 5,000,000 Base64 characters (~3.75 MB raw ciphertext).
    /// Sufficient for AES-256-GCM ciphertext of anonymized patient data.
    /// </summary>
    public const int MaxEncryptedPayloadLength = 5_000_000;
    /// <summary>
    /// Hybrid encapsulated key (ML-KEM-768 ciphertext ~1,088 bytes + X25519 ephemeral public key 32 bytes).
    /// Combined ~1,120 bytes (~1,494 Base64 chars). 3,000 provides headroom for framing/metadata.
    /// </summary>
    public const int MaxEncapsulatedKeyLength = 3000;
    /// <summary>
    /// Hybrid signature (ML-DSA-65 + ECDSA). ML-DSA-65 sig is ~3,309 bytes (~4,412 Base64 chars).
    /// </summary>
    public const int MaxSignatureLength = 8000;

    /// <summary>
    /// Validates standard pagination parameters (Page and PageSize).
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pagePropertyName">The property name for the page field.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="pageSizePropertyName">The property name for the page size field.</param>
    /// <param name="errors">The error list to append to.</param>
    internal static void ValidatePagination(
        int page, string pagePropertyName, int pageSize, string pageSizePropertyName, List<ValidationError> errors)
    {
        if (page < MinPage)
        {
            errors.Add(new ValidationError(pagePropertyName, $"Page must be at least {MinPage}."));
        }

        if (pageSize < 1 || pageSize > MaxPageSize)
        {
            errors.Add(new ValidationError(pageSizePropertyName, $"Page size must be between 1 and {MaxPageSize}."));
        }
    }

    /// <summary>
    /// Validates that a value is a non-empty, length-limited, valid Base64 string.
    /// Uses <see cref="Base64.IsValid(ReadOnlySpan{char})"/> to avoid allocating a decode buffer.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="propertyName">The property name for error reporting.</param>
    /// <param name="displayName">The human-readable field name.</param>
    /// <param name="maxLength">The maximum allowed string length.</param>
    /// <param name="errors">The error list to append to.</param>
    internal static void ValidateBase64Field(
        string? value, string propertyName, string displayName, int maxLength, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(propertyName, $"{displayName} is required."));
        }
        else if (value.Length > maxLength)
        {
            errors.Add(new ValidationError(propertyName, $"{displayName} must not exceed {maxLength} characters."));
        }
        else if (!Base64.IsValid(value.AsSpan()))
        {
            errors.Add(new ValidationError(propertyName, $"{displayName} must be a valid Base64 string."));
        }
    }
}
