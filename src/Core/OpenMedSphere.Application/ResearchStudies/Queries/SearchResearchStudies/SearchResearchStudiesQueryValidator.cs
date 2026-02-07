using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.ResearchStudies.Queries.SearchResearchStudies;

/// <summary>
/// Validates the <see cref="SearchResearchStudiesQuery"/>.
/// </summary>
internal sealed class SearchResearchStudiesQueryValidator : IValidator<SearchResearchStudiesQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(SearchResearchStudiesQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.ResearchArea is not null && instance.ResearchArea.Length > ValidationConstants.MaxResearchAreaLength)
        {
            errors.Add(new ValidationError(nameof(instance.ResearchArea), $"Research area must not exceed {ValidationConstants.MaxResearchAreaLength} characters."));
        }

        if (instance.TitleSearch is not null && instance.TitleSearch.Length > ValidationConstants.MaxSearchTextLength)
        {
            errors.Add(new ValidationError(nameof(instance.TitleSearch), $"Title search must not exceed {ValidationConstants.MaxSearchTextLength} characters."));
        }

        if (instance.Page < ValidationConstants.MinPage)
        {
            errors.Add(new ValidationError(nameof(instance.Page), $"Page must be at least {ValidationConstants.MinPage}."));
        }

        if (instance.PageSize < 1 || instance.PageSize > ValidationConstants.MaxPageSize)
        {
            errors.Add(new ValidationError(nameof(instance.PageSize), $"Page size must be between 1 and {ValidationConstants.MaxPageSize}."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
