using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

/// <summary>
/// Validates the <see cref="SearchPatientDataQuery"/>.
/// </summary>
internal sealed class SearchPatientDataQueryValidator : IValidator<SearchPatientDataQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(SearchPatientDataQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.DiagnosisText is not null && instance.DiagnosisText.Length > ValidationConstants.MaxSearchTextLength)
        {
            errors.Add(new ValidationError(nameof(instance.DiagnosisText), $"Diagnosis text must not exceed {ValidationConstants.MaxSearchTextLength} characters."));
        }

        if (instance.IcdCode is not null && instance.IcdCode.Length > ValidationConstants.MaxIcdCodeLength)
        {
            errors.Add(new ValidationError(nameof(instance.IcdCode), $"ICD code must not exceed {ValidationConstants.MaxIcdCodeLength} characters."));
        }

        if (instance.Region is not null && instance.Region.Length > ValidationConstants.MaxRegionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Region), $"Region must not exceed {ValidationConstants.MaxRegionLength} characters."));
        }

        if (instance.Page < ValidationConstants.MinPage)
        {
            errors.Add(new ValidationError(nameof(instance.Page), $"Page must be at least {ValidationConstants.MinPage}."));
        }

        if (instance.PageSize < 1 || instance.PageSize > ValidationConstants.MaxPageSize)
        {
            errors.Add(new ValidationError(nameof(instance.PageSize), $"Page size must be between 1 and {ValidationConstants.MaxPageSize}."));
        }

        if (instance.CollectedAfter.HasValue && instance.CollectedBefore.HasValue &&
            instance.CollectedBefore.Value < instance.CollectedAfter.Value)
        {
            errors.Add(new ValidationError(nameof(instance.CollectedBefore), "CollectedBefore must be on or after CollectedAfter."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
