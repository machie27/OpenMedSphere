using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.DataShares.Commands.RevokeDataShare;

/// <summary>
/// Handles the <see cref="RevokeDataShareCommand"/>.
/// </summary>
internal sealed class RevokeDataShareCommandHandler(
    IDataShareRepository repository,
    IUnitOfWork unitOfWork,
    IConcurrencyConflictDetector concurrencyDetector)
    : ICommandHandler<RevokeDataShareCommand>
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(
        RevokeDataShareCommand command,
        CancellationToken cancellationToken = default)
    {
        DataShare? dataShare = await repository.GetByIdAsync(command.DataShareId, cancellationToken);

        if (dataShare is null)
        {
            return Result.NotFound($"Data share with ID '{command.DataShareId}' not found.");
        }

        // Return NotFound (not Forbidden/InvalidOperation) to prevent share ID enumeration.
        if (dataShare.SenderResearcherId != command.ResearcherId)
        {
            return Result.NotFound($"Data share with ID '{command.DataShareId}' not found.");
        }

        if (dataShare.EffectiveStatus is DataShareStatus.Revoked)
        {
            return Result.InvalidOperation("Data share is already revoked.");
        }

        if (dataShare.EffectiveStatus is DataShareStatus.Expired)
        {
            return Result.InvalidOperation("Cannot revoke an expired data share.");
        }

        dataShare.Revoke();

        try
        {
            repository.Update(dataShare);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (concurrencyDetector.IsConcurrencyConflict(ex))
        {
            return Result.Conflict("The data share was modified concurrently. Please retry.");
        }

        return Result.Success();
    }
}
