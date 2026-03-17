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

        if (!sender.IsActive)
        {
            return Result<Guid>.InvalidOperation("Sender researcher is not active.");
        }

        var recipient = await researcherRepository.GetByIdAsync(command.RecipientResearcherId, cancellationToken);

        if (recipient is null || !recipient.IsActive)
        {
            return Result<Guid>.NotFound($"Recipient researcher with ID '{command.RecipientResearcherId}' not found.");
        }

        var patientData = await patientDataRepository.GetByIdAsync(command.PatientDataId, cancellationToken);

        if (patientData is null)
        {
            return Result<Guid>.NotFound($"Patient data with ID '{command.PatientDataId}' not found.");
        }

        // Key versions are validated before DataShare.Create to prevent stale-key encryption.
        // Note: a concurrent key rotation between the reads above and this check is possible
        // (small TOCTOU window), but the xmin concurrency token will cause SaveChanges to fail
        // if the researcher was modified in the interim.
        if (command.SenderKeyVersion != sender.PublicKeys.KeyVersion)
        {
            return Result<Guid>.InvalidOperation(
                "Sender key version is outdated. Fetch the latest public keys before encrypting.");
        }

        if (command.RecipientKeyVersion != recipient.PublicKeys.KeyVersion)
        {
            return Result<Guid>.InvalidOperation(
                "Recipient key version is outdated. Fetch the latest public keys before encrypting.");
        }

        // Checked at both validator and handler level to cover TOCTOU on expiry.
        if (command.ExpiresAtUtc.HasValue && command.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            return Result<Guid>.InvalidOperation("Expiry date must be in the future.");
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
