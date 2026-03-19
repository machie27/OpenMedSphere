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

        ValidationConstants.ValidateBase64Field(instance.MlKemPublicKey, nameof(instance.MlKemPublicKey), "ML-KEM public key", ValidationConstants.MaxBase64KeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.MlDsaPublicKey, nameof(instance.MlDsaPublicKey), "ML-DSA public key", ValidationConstants.MaxBase64KeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.X25519PublicKey, nameof(instance.X25519PublicKey), "X25519 public key", ValidationConstants.MaxBase64KeyLength, errors);
        ValidationConstants.ValidateBase64Field(instance.EcdsaPublicKey, nameof(instance.EcdsaPublicKey), "ECDSA public key", ValidationConstants.MaxBase64KeyLength, errors);

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
