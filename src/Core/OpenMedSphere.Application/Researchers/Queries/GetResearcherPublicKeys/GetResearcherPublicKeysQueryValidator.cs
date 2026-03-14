using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.Researchers.Queries.GetResearcherPublicKeys;

/// <summary>
/// Validates the <see cref="GetResearcherPublicKeysQuery"/>.
/// </summary>
internal sealed class GetResearcherPublicKeysQueryValidator : IValidator<GetResearcherPublicKeysQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(GetResearcherPublicKeysQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.Id), "Researcher ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
