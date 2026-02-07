using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Commands.AnonymizePatientData;

/// <summary>
/// Validates the <see cref="AnonymizePatientDataCommand"/>.
/// </summary>
internal sealed class AnonymizePatientDataCommandValidator : IValidator<AnonymizePatientDataCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(AnonymizePatientDataCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.PatientDataId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.PatientDataId), "Patient data ID is required."));
        }

        if (instance.PolicyId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.PolicyId), "Policy ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
