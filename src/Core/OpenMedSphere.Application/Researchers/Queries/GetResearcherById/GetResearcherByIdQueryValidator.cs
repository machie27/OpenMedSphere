using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.GetResearcherById;

/// <summary>
/// Validates the <see cref="GetResearcherByIdQuery"/>.
/// </summary>
internal sealed class GetResearcherByIdQueryValidator : IValidator<GetResearcherByIdQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(GetResearcherByIdQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.Id), "Researcher ID is required."));
        }

        if (instance.CallerId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.CallerId), "Caller ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
