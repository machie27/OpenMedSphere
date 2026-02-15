using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.DataShares.Commands.CreateDataShare;

/// <summary>
/// Handles the <see cref="CreateDataShareCommand"/>.
/// </summary>
internal sealed class CreateDataShareCommandHandler(
    IDataShareRepository dataShareRepository,
    IResearcherRepository researcherRepository,
    IPatientDataRepository patientDataRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDataShareCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(
        CreateDataShareCommand command,
        CancellationToken cancellationToken = default)
    {
        var sender = await researcherRepository.GetByIdAsync(command.SenderResearcherId, cancellationToken);

        if (sender is null)
        {
            return Result<Guid>.NotFound($"Sender researcher with ID '{command.SenderResearcherId}' not found.");
        }

        var recipient = await researcherRepository.GetByIdAsync(command.RecipientResearcherId, cancellationToken);

        if (recipient is null)
        {
            return Result<Guid>.NotFound($"Recipient researcher with ID '{command.RecipientResearcherId}' not found.");
        }

        Domain.Entities.PatientData? patientData =
            await patientDataRepository.GetByIdAsync(command.PatientDataId, cancellationToken);

        if (patientData is null)
        {
            return Result<Guid>.NotFound($"Patient data with ID '{command.PatientDataId}' not found.");
        }

        var dataShare = DataShare.Create(
            command.SenderResearcherId,
            command.RecipientResearcherId,
            command.PatientDataId,
            command.EncryptedPayload,
            command.EncapsulatedKey,
            command.Signature,
            command.SenderKeyVersion,
            command.RecipientKeyVersion,
            command.ExpiresAtUtc);

        await dataShareRepository.AddAsync(dataShare, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(dataShare.Id);
    }
}
