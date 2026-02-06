using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

/// <summary>
/// Validates the <see cref="SearchPatientDataQuery"/>.
/// </summary>
internal sealed class SearchPatientDataQueryValidator : IValidator<SearchPatientDataQuery>
{
    /// <inheritdoc />
    public ValidationResult Validate(SearchPatientDataQuery instance)
    {
        List<ValidationError> errors = [];

        if (instance.DiagnosisText is not null && instance.DiagnosisText.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.DiagnosisText), "Diagnosis text must not exceed 200 characters."));
        }

        if (instance.IcdCode is not null && instance.IcdCode.Length > 50)
        {
            errors.Add(new ValidationError(nameof(instance.IcdCode), "ICD code must not exceed 50 characters."));
        }

        if (instance.Region is not null && instance.Region.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.Region), "Region must not exceed 200 characters."));
        }

        if (instance.Page < 1)
        {
            errors.Add(new ValidationError(nameof(instance.Page), "Page must be at least 1."));
        }

        if (instance.PageSize < 1 || instance.PageSize > 100)
        {
            errors.Add(new ValidationError(nameof(instance.PageSize), "Page size must be between 1 and 100."));
        }

        if (instance.CollectedAfter.HasValue && instance.CollectedBefore.HasValue &&
            instance.CollectedBefore.Value < instance.CollectedAfter.Value)
        {
            errors.Add(new ValidationError(nameof(instance.CollectedBefore), "CollectedBefore must be on or after CollectedAfter."));
        }

        return errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors };
    }
}
