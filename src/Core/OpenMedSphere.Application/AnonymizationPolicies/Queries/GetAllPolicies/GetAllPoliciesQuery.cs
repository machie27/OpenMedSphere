using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.AnonymizationPolicies.Queries.GetAllPolicies;

/// <summary>
/// Query to get all active anonymization policies.
/// </summary>
public sealed record GetAllPoliciesQuery : IQuery<IReadOnlyList<AnonymizationPolicyResponse>>;
