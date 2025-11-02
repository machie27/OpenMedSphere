namespace OpenMedSphere.Domain.Enums;

/// <summary>
/// Defines the levels of anonymization that can be applied to patient data.
/// </summary>
public enum AnonymizationLevel
{
    /// <summary>
    /// No anonymization applied. Data remains in its original form.
    /// </summary>
    None = 0,

    /// <summary>
    /// Basic anonymization. Removes direct identifiers such as name, address, and SSN.
    /// </summary>
    Basic = 1,

    /// <summary>
    /// Standard anonymization. Removes direct identifiers and generalizes quasi-identifiers
    /// such as date of birth (year only) and zip code (first 3 digits).
    /// </summary>
    Standard = 2,

    /// <summary>
    /// Advanced anonymization. Applies k-anonymity, generalization, and suppression techniques
    /// to ensure individual records cannot be re-identified.
    /// </summary>
    Advanced = 3,

    /// <summary>
    /// Full anonymization. Applies differential privacy techniques and maximum generalization.
    /// Provides the highest level of privacy protection.
    /// </summary>
    Full = 4
}
