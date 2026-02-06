using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.AnonymizationPolicies.Commands.CreatePolicy;

/// <summary>
/// Handles the <see cref="CreateAnonymizationPolicyCommand"/>.
/// </summary>
internal sealed class CreateAnonymizationPolicyCommandHandler(
    IAnonymizationPolicyRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateAnonymizationPolicyCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(
        CreateAnonymizationPolicyCommand command,
        CancellationToken cancellationToken = default)
    {
        AnonymizationPolicy policy = AnonymizationPolicy.Create(
            command.Name,
            command.Level,
            command.Description);

        await repository.AddAsync(policy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(policy.Id);
    }
}
