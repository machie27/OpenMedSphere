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
    IUnitOfWork unitOfWork,
    IConcurrencyConflictDetector concurrencyDetector)
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

        if (!researcher.IsActive)
        {
            return Result.InvalidOperation("Cannot update keys for an inactive researcher account.");
        }

        if (command.KeyVersion <= researcher.PublicKeys.KeyVersion)
        {
            return Result.InvalidOperation(
                "New key version must be greater than the current version.");
        }

        var newPublicKeys = PublicKeySet.Create(
            command.MlKemPublicKey,
            command.MlDsaPublicKey,
            command.X25519PublicKey,
            command.EcdsaPublicKey,
            command.KeyVersion);

        researcher.RotateKeys(newPublicKeys);

        try
        {
            repository.Update(researcher);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (concurrencyDetector.IsConcurrencyConflict(ex))
        {
            return Result.Conflict("The researcher was modified concurrently. Please retry.");
        }

        return Result.Success();
    }
}
