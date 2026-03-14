using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.SearchResearchers;

/// <summary>
/// Validates the <see cref="SearchResearchersQuery"/>.
/// </summary>
internal sealed class SearchResearchersQueryValidator : IValidator<SearchResearchersQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(SearchResearchersQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (string.IsNullOrWhiteSpace(instance.Query))
        {
            errors.Add(new ValidationError(nameof(instance.Query), "Search query is required."));
        }
        else if (instance.Query.Length > ValidationConstants.MaxSearchTextLength)
        {
            errors.Add(new ValidationError(nameof(instance.Query), $"Search query must not exceed {ValidationConstants.MaxSearchTextLength} characters."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
