using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Commands.CreatePatientData;

/// <summary>
/// Validates the <see cref="CreatePatientDataCommand"/>.
/// </summary>
internal sealed class CreatePatientDataCommandValidator : IValidator<CreatePatientDataCommand>
{
    /// <inheritdoc />
    public ValidationResult Validate(CreatePatientDataCommand instance)
    {
        List<ValidationError> errors = [];

        if (instance.YearOfBirth.HasValue)
        {
            if (instance.YearOfBirth.Value < 1900)
            {
                errors.Add(new ValidationError(nameof(instance.YearOfBirth), "Year of birth must be 1900 or later."));
            }
            else if (instance.YearOfBirth.Value > DateTime.UtcNow.Year)
            {
                errors.Add(new ValidationError(nameof(instance.YearOfBirth), "Year of birth cannot be in the future."));
            }
        }

        if (instance.Gender is not null && instance.Gender.Length > 50)
        {
            errors.Add(new ValidationError(nameof(instance.Gender), "Gender must not exceed 50 characters."));
        }

        if (instance.Region is not null && instance.Region.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.Region), "Region must not exceed 200 characters."));
        }

        if (instance.PrimaryDiagnosis is not null && instance.PrimaryDiagnosis.Length > 500)
        {
            errors.Add(new ValidationError(nameof(instance.PrimaryDiagnosis), "Primary diagnosis must not exceed 500 characters."));
        }

        if (instance.PrimaryDiagnosisIcdCode is not null && instance.PrimaryDiagnosisIcdCode.Length > 50)
        {
            errors.Add(new ValidationError(nameof(instance.PrimaryDiagnosisIcdCode), "ICD code must not exceed 50 characters."));
        }

        if (instance.ClinicalNotes is not null && instance.ClinicalNotes.Length > 10000)
        {
            errors.Add(new ValidationError(nameof(instance.ClinicalNotes), "Clinical notes must not exceed 10000 characters."));
        }

        if (instance.SecondaryDiagnoses is not null && instance.SecondaryDiagnoses.Count > 50)
        {
            errors.Add(new ValidationError(nameof(instance.SecondaryDiagnoses), "Secondary diagnoses list must not exceed 50 items."));
        }

        if (instance.Medications is not null && instance.Medications.Count > 100)
        {
            errors.Add(new ValidationError(nameof(instance.Medications), "Medications list must not exceed 100 items."));
        }

        return errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors };
    }
}
