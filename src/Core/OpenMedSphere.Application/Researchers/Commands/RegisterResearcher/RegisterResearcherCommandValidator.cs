using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Commands.RegisterResearcher;

/// <summary>
/// Validates the <see cref="RegisterResearcherCommand"/>.
/// </summary>
internal sealed class RegisterResearcherCommandValidator : IValidator<RegisterResearcherCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(RegisterResearcherCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (string.IsNullOrWhiteSpace(instance.ExternalId))
        {
            errors.Add(new ValidationError(nameof(instance.ExternalId), "External identity is required."));
        }
        else if (instance.ExternalId.Length > ValidationConstants.MaxNameLength)
        {
            errors.Add(new ValidationError(nameof(instance.ExternalId), $"External identity must not exceed {ValidationConstants.MaxNameLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            errors.Add(new ValidationError(nameof(instance.Name), "Name is required."));
        }
        else if (instance.Name.Length > ValidationConstants.MaxNameLength)
        {
            errors.Add(new ValidationError(nameof(instance.Name), $"Name must not exceed {ValidationConstants.MaxNameLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Email))
        {
            errors.Add(new ValidationError(nameof(instance.Email), "Email is required."));
        }
        else if (instance.Email.Length > ValidationConstants.MaxEmailLength)
        {
            errors.Add(new ValidationError(nameof(instance.Email), $"Email must not exceed {ValidationConstants.MaxEmailLength} characters."));
        }
        else if (!System.Net.Mail.MailAddress.TryCreate(instance.Email, out _))
        {
            errors.Add(new ValidationError(nameof(instance.Email), "Email must be a valid email address."));
        }

        if (string.IsNullOrWhiteSpace(instance.Institution))
        {
            errors.Add(new ValidationError(nameof(instance.Institution), "Institution is required."));
        }
        else if (instance.Institution.Length > ValidationConstants.MaxInstitutionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Institution), $"Institution must not exceed {ValidationConstants.MaxInstitutionLength} characters."));
        }

        ValidationConstants.ValidateBase64Field(instance.MlKemPublicKey, nameof(instance.MlKemPublicKey), "ML-KEM public key", ValidationConstants.MaxBase64KeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.MlDsaPublicKey, nameof(instance.MlDsaPublicKey), "ML-DSA public key", ValidationConstants.MaxBase64KeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.X25519PublicKey, nameof(instance.X25519PublicKey), "X25519 public key", ValidationConstants.MaxBase64KeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.EcdsaPublicKey, nameof(instance.EcdsaPublicKey), "ECDSA public key", ValidationConstants.MaxBase64KeyLength, errors);

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
