using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Enums;

namespace OpenMedSphere.Application.AnonymizationPolicies.Commands.CreatePolicy;

/// <summary>
/// Validates the <see cref="CreateAnonymizationPolicyCommand"/>.
/// </summary>
internal sealed class CreateAnonymizationPolicyCommandValidator : IValidator<CreateAnonymizationPolicyCommand>
{
    /// <inheritdoc />
    public ValidationResult Validate(CreateAnonymizationPolicyCommand instance)
    {
        List<ValidationError> errors = [];

        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            errors.Add(new ValidationError(nameof(instance.Name), "Name is required."));
        }
        else if (instance.Name.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.Name), "Name must not exceed 200 characters."));
        }

        if (instance.Description is not null && instance.Description.Length > 2000)
        {
            errors.Add(new ValidationError(nameof(instance.Description), "Description must not exceed 2000 characters."));
        }

        if (!Enum.IsDefined(instance.Level))
        {
            errors.Add(new ValidationError(nameof(instance.Level), "Invalid anonymization level."));
        }

        return errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors };
    }
}
