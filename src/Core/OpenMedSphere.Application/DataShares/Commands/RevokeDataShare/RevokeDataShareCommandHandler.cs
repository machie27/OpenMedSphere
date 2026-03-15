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
    IUnitOfWork unitOfWork)
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

        if (dataShare.SenderResearcherId != command.ResearcherId)
        {
            return Result.InvalidOperation("Only the sender can revoke a data share.");
        }

        if (dataShare.Status is DataShareStatus.Revoked)
        {
            return Result.InvalidOperation("Data share is already revoked.");
        }

        if (dataShare.Status is DataShareStatus.Pending && dataShare.IsExpired())
        {
            return Result.InvalidOperation("Cannot revoke an expired data share.");
        }

        dataShare.Revoke();

        repository.Update(dataShare);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
