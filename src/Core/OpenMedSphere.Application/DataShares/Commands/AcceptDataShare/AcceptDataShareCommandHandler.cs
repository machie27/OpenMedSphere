using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

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

        if (dataShare.RecipientResearcherId != command.ResearcherId)
        {
            return Result.InvalidOperation("Only the recipient can accept a data share.");
        }

        dataShare.Accept();

        repository.Update(dataShare);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
