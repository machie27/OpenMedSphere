using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Commands.UpdateResearcherPublicKeys;

/// <summary>
/// Validates the <see cref="UpdateResearcherPublicKeysCommand"/>.
/// </summary>
internal sealed class UpdateResearcherPublicKeysCommandValidator : IValidator<UpdateResearcherPublicKeysCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(UpdateResearcherPublicKeysCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.ResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.ResearcherId), "Researcher ID is required."));
        }

        if (instance.KeyVersion < 1)
        {
            errors.Add(new ValidationError(nameof(instance.KeyVersion), "Key version must be at least 1."));
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
