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
    IUnitOfWork unitOfWork)
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

        if (dataShare.Status is not DataShareStatus.Pending)
        {
            return Result.InvalidOperation($"Cannot accept a data share with status '{dataShare.Status}'.");
        }

        if (dataShare.IsExpired())
        {
            return Result.InvalidOperation("Cannot accept an expired data share.");
        }

        dataShare.Accept();

        repository.Update(dataShare);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
