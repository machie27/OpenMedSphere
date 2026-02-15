using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.Researchers.Commands.UpdateResearcherPublicKeys;

/// <summary>
/// Handles the <see cref="UpdateResearcherPublicKeysCommand"/>.
/// </summary>
internal sealed class UpdateResearcherPublicKeysCommandHandler(
    IResearcherRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateResearcherPublicKeysCommand>
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(
        UpdateResearcherPublicKeysCommand command,
        CancellationToken cancellationToken = default)
    {
        var researcher = await repository.GetByIdAsync(command.ResearcherId, cancellationToken);

        if (researcher is null)
        {
            return Result.NotFound($"Researcher with ID '{command.ResearcherId}' not found.");
        }

        var newPublicKeys = PublicKeySet.Create(
            command.MlKemPublicKey,
            command.MlDsaPublicKey,
            command.X25519PublicKey,
            command.EcdsaPublicKey,
            command.KeyVersion);

        researcher.RotateKeys(newPublicKeys);

        repository.Update(researcher);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
