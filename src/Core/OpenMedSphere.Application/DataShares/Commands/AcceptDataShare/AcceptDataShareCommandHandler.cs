using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.DataShares.Commands.AcceptDataShare;

/// <summary>
/// Handles the <see cref="AcceptDataShareCommand"/>.
/// </summary>
internal sealed class AcceptDataShareCommandHandler(
    IDataShareRepository repository,
    IUnitOfWork unitOfWork,
    IConcurrencyConflictDetector concurrencyDetector)
    : ICommandHandler<AcceptDataShareCommand>
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(
        AcceptDataShareCommand command,
        CancellationToken cancellationToken = default)
    {
        DataShare? dataShare = await repository.GetByIdAsync(command.DataShareId, cancellationToken);

        if (dataShare is null)
        {
            return Result.NotFound($"Data share with ID '{command.DataShareId}' not found.");
        }

        // Return NotFound (not Forbidden/InvalidOperation) to prevent share ID enumeration.
        if (dataShare.RecipientResearcherId != command.ResearcherId)
        {
            return Result.NotFound($"Data share with ID '{command.DataShareId}' not found.");
        }

        if (dataShare.EffectiveStatus is not DataShareStatus.Pending)
        {
            return Result.InvalidOperation($"Cannot accept a data share with status '{dataShare.EffectiveStatus}'.");
        }

        dataShare.Accept();

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
