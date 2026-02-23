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

        if (string.IsNullOrWhiteSpace(instance.Institution))
        {
            errors.Add(new ValidationError(nameof(instance.Institution), "Institution is required."));
        }
        else if (instance.Institution.Length > ValidationConstants.MaxInstitutionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Institution), $"Institution must not exceed {ValidationConstants.MaxInstitutionLength} characters."));
        }

        ValidateBase64Key(instance.MlKemPublicKey, nameof(instance.MlKemPublicKey), "ML-KEM public key", errors);
        ValidateBase64Key(instance.MlDsaPublicKey, nameof(instance.MlDsaPublicKey), "ML-DSA public key", errors);
        ValidateBase64Key(instance.X25519PublicKey, nameof(instance.X25519PublicKey), "X25519 public key", errors);
        ValidateBase64Key(instance.EcdsaPublicKey, nameof(instance.EcdsaPublicKey), "ECDSA public key", errors);

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }

    private static void ValidateBase64Key(string? value, string propertyName, string displayName, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(propertyName, $"{displayName} is required."));
        }
        else if (value.Length > ValidationConstants.MaxBase64KeyLength)
        {
            errors.Add(new ValidationError(propertyName, $"{displayName} must not exceed {ValidationConstants.MaxBase64KeyLength} characters."));
        }
    }
}
