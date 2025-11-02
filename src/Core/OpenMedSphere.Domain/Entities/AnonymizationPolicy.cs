using OpenMedSphere.Domain.Enums;
using OpenMedSphere.Domain.Primitives;

namespace OpenMedSphere.Domain.Entities;

/// <summary>
/// Represents a policy that defines how patient data should be anonymized.
/// This is an aggregate root that encapsulates anonymization rules and constraints.
/// </summary>
public sealed class AnonymizationPolicy : AggregateRoot<Guid>
{
    /// <summary>
    /// Gets the name of the anonymization policy.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets the description of the anonymization policy.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets the anonymization level applied by this policy.
    /// </summary>
    public required AnonymizationLevel Level { get; set; }

    /// <summary>
    /// Gets a value indicating whether date of birth should be generalized (year only).
    /// </summary>
    public bool GeneralizeDateOfBirth { get; set; }

    /// <summary>
    /// Gets a value indicating whether location data should be generalized (e.g., zip code to region).
    /// </summary>
    public bool GeneralizeLocation { get; set; }

    /// <summary>
    /// Gets a value indicating whether rare diagnoses should be suppressed or generalized.
    /// </summary>
    public bool SuppressRareDiagnoses { get; set; }

    /// <summary>
    /// Gets the minimum group size for k-anonymity (if applicable).
    /// </summary>
    public int? KAnonymityThreshold { get; set; }

    /// <summary>
    /// Gets the date and time when the policy was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Gets the date and time when the policy was last updated.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Gets a value indicating whether the policy is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Required for EF Core.
    /// </summary>
    private AnonymizationPolicy() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymizationPolicy"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the policy.</param>
    private AnonymizationPolicy(Guid id) : base(id)
    {
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new anonymization policy.
    /// </summary>
    /// <param name="name">The name of the policy.</param>
    /// <param name="level">The anonymization level.</param>
    /// <param name="description">The description of the policy.</param>
    /// <returns>A new anonymization policy.</returns>
    public static AnonymizationPolicy Create(
        string name,
        AnonymizationLevel level,
        string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new AnonymizationPolicy(Guid.NewGuid())
        {
            Name = name,
            Level = level,
            Description = description,
            GeneralizeDateOfBirth = level >= AnonymizationLevel.Standard,
            GeneralizeLocation = level >= AnonymizationLevel.Standard,
            SuppressRareDiagnoses = level >= AnonymizationLevel.Advanced,
            KAnonymityThreshold = level >= AnonymizationLevel.Advanced ? 5 : null
        };
    }

    /// <summary>
    /// Updates the policy configuration.
    /// </summary>
    /// <param name="name">The new name for the policy.</param>
    /// <param name="description">The new description.</param>
    /// <param name="level">The new anonymization level.</param>
    public void Update(string name, string? description, AnonymizationLevel level)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Description = description;
        Level = level;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Configures the k-anonymity threshold for this policy.
    /// </summary>
    /// <param name="threshold">The minimum group size for k-anonymity.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when threshold is less than 2.</exception>
    public void ConfigureKAnonymity(int threshold)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threshold, 2);

        KAnonymityThreshold = threshold;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Configures the anonymization options for this policy.
    /// </summary>
    /// <param name="generalizeDateOfBirth">Whether to generalize date of birth.</param>
    /// <param name="generalizeLocation">Whether to generalize location data.</param>
    /// <param name="suppressRareDiagnoses">Whether to suppress rare diagnoses.</param>
    public void ConfigureOptions(
        bool generalizeDateOfBirth,
        bool generalizeLocation,
        bool suppressRareDiagnoses)
    {
        GeneralizeDateOfBirth = generalizeDateOfBirth;
        GeneralizeLocation = generalizeLocation;
        SuppressRareDiagnoses = suppressRareDiagnoses;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the policy.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the policy.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
