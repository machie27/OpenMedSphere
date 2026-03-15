using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Commands.CreateDataShare;

/// <summary>
/// Validates the <see cref="CreateDataShareCommand"/>.
/// </summary>
internal sealed class CreateDataShareCommandValidator : IValidator<CreateDataShareCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(CreateDataShareCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.SenderResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.SenderResearcherId), "Sender researcher ID is required."));
        }

        if (instance.RecipientResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.RecipientResearcherId), "Recipient researcher ID is required."));
        }

        if (instance.SenderResearcherId == instance.RecipientResearcherId && instance.SenderResearcherId != Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.RecipientResearcherId), "Sender and recipient must be different researchers."));
        }

        if (instance.PatientDataId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.PatientDataId), "Patient data ID is required."));
        }

        ValidationConstants.ValidateBase64Field(instance.EncryptedPayload, nameof(instance.EncryptedPayload), "Encrypted payload", ValidationConstants.MaxEncryptedPayloadLength, errors);
        ValidationConstants.ValidateBase64Field(instance.EncapsulatedKey, nameof(instance.EncapsulatedKey), "Encapsulated key", ValidationConstants.MaxEncapsulatedKeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.Signature, nameof(instance.Signature), "Signature", ValidationConstants.MaxSignatureLength, errors);

        if (instance.SenderKeyVersion < 1)
        {
            errors.Add(new ValidationError(nameof(instance.SenderKeyVersion), "Sender key version must be at least 1."));
        }

        if (instance.RecipientKeyVersion < 1)
        {
            errors.Add(new ValidationError(nameof(instance.RecipientKeyVersion), "Recipient key version must be at least 1."));
        }

        if (instance.ExpiresAtUtc.HasValue && instance.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            errors.Add(new ValidationError(nameof(instance.ExpiresAtUtc), "Expiry date must be in the future."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
