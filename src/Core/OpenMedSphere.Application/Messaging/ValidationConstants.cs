namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Shared validation constants used across validators.
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
    public const int MaxEmailLength = 320;
    /// <summary>
    /// Max Base64-encoded public key size. ML-DSA-65 is the largest at ~2,604 chars;
    /// 4,000 provides comfortable headroom for all key types.
    /// </summary>
    public const int MaxBase64KeyLength = 4000;
    /// <summary>
    /// 5 MB — sufficient for AES-256-GCM ciphertext of anonymized patient data.
    /// </summary>
    public const int MaxEncryptedPayloadLength = 5_000_000;
    /// <summary>
    /// ML-KEM-768 encapsulated key is ~1,088 bytes (~1,452 Base64 chars).
    /// </summary>
    public const int MaxEncapsulatedKeyLength = 3000;
    /// <summary>
    /// Hybrid signature (ML-DSA-65 + ECDSA). ML-DSA-65 sig is ~3,309 bytes (~4,412 Base64 chars).
    /// </summary>
    public const int MaxSignatureLength = 8000;
}
