using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.AnonymizationPolicies.Commands.CreatePolicy;

/// <summary>
/// Command to create a new anonymization policy.
/// </summary>
public sealed record CreateAnonymizationPolicyCommand : ICommand<Guid>
{
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
    public required AnonymizationLevel Level { get; init; }
}
