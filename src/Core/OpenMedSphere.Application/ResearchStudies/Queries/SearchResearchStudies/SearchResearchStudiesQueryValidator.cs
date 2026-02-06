using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.ResearchStudies.Queries.SearchResearchStudies;

/// <summary>
/// Validates the <see cref="SearchResearchStudiesQuery"/>.
/// </summary>
internal sealed class SearchResearchStudiesQueryValidator : IValidator<SearchResearchStudiesQuery>
{
    /// <inheritdoc />
    public ValidationResult Validate(SearchResearchStudiesQuery instance)
    {
        List<ValidationError> errors = [];

        if (instance.ResearchArea is not null && instance.ResearchArea.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.ResearchArea), "Research area must not exceed 200 characters."));
        }

        if (instance.TitleSearch is not null && instance.TitleSearch.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.TitleSearch), "Title search must not exceed 200 characters."));
        }

        if (instance.Page < 1)
        {
            errors.Add(new ValidationError(nameof(instance.Page), "Page must be at least 1."));
        }

        if (instance.PageSize < 1 || instance.PageSize > 100)
        {
            errors.Add(new ValidationError(nameof(instance.PageSize), "Page size must be between 1 and 100."));
        }

        return errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors };
    }
}
