using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Commands.CreatePatientData;

/// <summary>
/// Validates the <see cref="CreatePatientDataCommand"/>.
/// </summary>
internal sealed class CreatePatientDataCommandValidator : IValidator<CreatePatientDataCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(CreatePatientDataCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.YearOfBirth.HasValue)
        {
            if (instance.YearOfBirth.Value < ValidationConstants.MinYearOfBirth)
            {
                errors.Add(new ValidationError(nameof(instance.YearOfBirth), $"Year of birth must be {ValidationConstants.MinYearOfBirth} or later."));
            }
            else if (instance.YearOfBirth.Value > DateTime.UtcNow.Year)
            {
                errors.Add(new ValidationError(nameof(instance.YearOfBirth), "Year of birth cannot be in the future."));
            }
        }

        if (instance.Gender is not null && instance.Gender.Length > ValidationConstants.MaxGenderLength)
        {
            errors.Add(new ValidationError(nameof(instance.Gender), $"Gender must not exceed {ValidationConstants.MaxGenderLength} characters."));
        }

        if (instance.Region is not null && instance.Region.Length > ValidationConstants.MaxRegionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Region), $"Region must not exceed {ValidationConstants.MaxRegionLength} characters."));
        }

        if (instance.PrimaryDiagnosis is not null && instance.PrimaryDiagnosis.Length > ValidationConstants.MaxDiagnosisLength)
        {
            errors.Add(new ValidationError(nameof(instance.PrimaryDiagnosis), $"Primary diagnosis must not exceed {ValidationConstants.MaxDiagnosisLength} characters."));
        }

        if (instance.PrimaryDiagnosisIcdCode is not null && instance.PrimaryDiagnosisIcdCode.Length > ValidationConstants.MaxIcdCodeLength)
        {
            errors.Add(new ValidationError(nameof(instance.PrimaryDiagnosisIcdCode), $"ICD code must not exceed {ValidationConstants.MaxIcdCodeLength} characters."));
        }

        if (instance.ClinicalNotes is not null && instance.ClinicalNotes.Length > ValidationConstants.MaxNotesLength)
        {
            errors.Add(new ValidationError(nameof(instance.ClinicalNotes), $"Clinical notes must not exceed {ValidationConstants.MaxNotesLength} characters."));
        }

        if (instance.SecondaryDiagnoses is not null && instance.SecondaryDiagnoses.Count > ValidationConstants.MaxSecondaryDiagnoses)
        {
            errors.Add(new ValidationError(nameof(instance.SecondaryDiagnoses), $"Secondary diagnoses list must not exceed {ValidationConstants.MaxSecondaryDiagnoses} items."));
        }

        if (instance.Medications is not null && instance.Medications.Count > ValidationConstants.MaxMedications)
        {
            errors.Add(new ValidationError(nameof(instance.Medications), $"Medications list must not exceed {ValidationConstants.MaxMedications} items."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
