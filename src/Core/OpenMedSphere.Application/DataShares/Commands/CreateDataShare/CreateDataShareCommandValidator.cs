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

        if (string.IsNullOrWhiteSpace(instance.EncryptedPayload))
        {
            errors.Add(new ValidationError(nameof(instance.EncryptedPayload), "Encrypted payload is required."));
        }
        else if (instance.EncryptedPayload.Length > ValidationConstants.MaxEncryptedPayloadLength)
        {
            errors.Add(new ValidationError(nameof(instance.EncryptedPayload), $"Encrypted payload must not exceed {ValidationConstants.MaxEncryptedPayloadLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.EncapsulatedKey))
        {
            errors.Add(new ValidationError(nameof(instance.EncapsulatedKey), "Encapsulated key is required."));
        }
        else if (instance.EncapsulatedKey.Length > ValidationConstants.MaxEncapsulatedKeyLength)
        {
            errors.Add(new ValidationError(nameof(instance.EncapsulatedKey), $"Encapsulated key must not exceed {ValidationConstants.MaxEncapsulatedKeyLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Signature))
        {
            errors.Add(new ValidationError(nameof(instance.Signature), "Signature is required."));
        }
        else if (instance.Signature.Length > ValidationConstants.MaxSignatureLength)
        {
            errors.Add(new ValidationError(nameof(instance.Signature), $"Signature must not exceed {ValidationConstants.MaxSignatureLength} characters."));
        }

        if (instance.SenderKeyVersion < 1)
        {
            errors.Add(new ValidationError(nameof(instance.SenderKeyVersion), "Sender key version must be at least 1."));
        }

        if (instance.RecipientKeyVersion < 1)
        {
            errors.Add(new ValidationError(nameof(instance.RecipientKeyVersion), "Recipient key version must be at least 1."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
