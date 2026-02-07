using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.AnonymizationPolicies.Commands.CreatePolicy;

/// <summary>
/// Validates the <see cref="CreateAnonymizationPolicyCommand"/>.
/// </summary>
internal sealed class CreateAnonymizationPolicyCommandValidator : IValidator<CreateAnonymizationPolicyCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(CreateAnonymizationPolicyCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            errors.Add(new ValidationError(nameof(instance.Name), "Name is required."));
        }
        else if (instance.Name.Length > ValidationConstants.MaxPolicyNameLength)
        {
            errors.Add(new ValidationError(nameof(instance.Name), $"Name must not exceed {ValidationConstants.MaxPolicyNameLength} characters."));
        }

        if (instance.Description is not null && instance.Description.Length > ValidationConstants.MaxPolicyDescriptionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Description), $"Description must not exceed {ValidationConstants.MaxPolicyDescriptionLength} characters."));
        }

        if (!Enum.IsDefined(instance.Level))
        {
            errors.Add(new ValidationError(nameof(instance.Level), "Invalid anonymization level."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
