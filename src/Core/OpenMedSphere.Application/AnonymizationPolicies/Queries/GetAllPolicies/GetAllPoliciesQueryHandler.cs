using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.AnonymizationPolicies.Queries.GetAllPolicies;

/// <summary>
/// Handles the <see cref="GetAllPoliciesQuery"/>.
/// </summary>
internal sealed class GetAllPoliciesQueryHandler(IAnonymizationPolicyRepository repository)
    : IQueryHandler<GetAllPoliciesQuery, IReadOnlyList<AnonymizationPolicyResponse>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AnonymizationPolicyResponse>>> HandleAsync(
        GetAllPoliciesQuery query,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AnonymizationPolicy> policies =
            await repository.GetActivePoliciesAsync(cancellationToken);

        IReadOnlyList<AnonymizationPolicyResponse> responses = policies
            .Select(p => new AnonymizationPolicyResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Level = p.Level,
                GeneralizeDateOfBirth = p.GeneralizeDateOfBirth,
                GeneralizeLocation = p.GeneralizeLocation,
                SuppressRareDiagnoses = p.SuppressRareDiagnoses,
                KAnonymityThreshold = p.KAnonymityThreshold,
                IsActive = p.IsActive,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<AnonymizationPolicyResponse>>.Success(responses);
    }
}
