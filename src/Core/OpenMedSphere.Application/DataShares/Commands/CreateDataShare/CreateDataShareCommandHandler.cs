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
        // Note: a small TOCTOU window exists between the key version checks above and the
        // insert below. A concurrent key rotation could complete in this window, leaving the
        // stored SenderKeyVersion/RecipientKeyVersion stale. This is accepted as a low-probability
        // edge case — the xmin token does NOT protect here because we are inserting DataShare,
        // not updating Researcher. A client retrying with fresh key data will create a new share
        // if needed.
        // Crucially, this is NOT a silent security failure: the payload is already encrypted
        // client-side before this request. A stale key version means the recipient will fail to
        // decrypt (wrong key), resulting in a useless share — not a compromised one.
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
