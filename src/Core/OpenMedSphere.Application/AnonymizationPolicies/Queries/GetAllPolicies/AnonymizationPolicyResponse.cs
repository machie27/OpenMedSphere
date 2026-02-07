using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.AnonymizationPolicies.Queries.GetAllPolicies;

/// <summary>
/// Response DTO for anonymization policies.
/// </summary>
public sealed record AnonymizationPolicyResponse
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the anonymization level.
    /// </summary>
    public AnonymizationLevel Level { get; init; }

    /// <summary>
    /// Gets whether date of birth is generalized.
    /// </summary>
    public bool GeneralizeDateOfBirth { get; init; }

    /// <summary>
    /// Gets whether location is generalized.
    /// </summary>
    public bool GeneralizeLocation { get; init; }

    /// <summary>
    /// Gets whether rare diagnoses are suppressed.
    /// </summary>
    public bool SuppressRareDiagnoses { get; init; }

    /// <summary>
    /// Gets the k-anonymity threshold.
    /// </summary>
    public int? KAnonymityThreshold { get; init; }

    /// <summary>
    /// Gets whether the policy is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }
}
