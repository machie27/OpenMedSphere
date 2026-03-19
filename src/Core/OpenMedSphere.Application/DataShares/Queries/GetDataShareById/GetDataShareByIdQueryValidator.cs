using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Queries.GetDataShareById;

/// <summary>
/// Validates the <see cref="GetDataShareByIdQuery"/>.
/// </summary>
internal sealed class GetDataShareByIdQueryValidator : IValidator<GetDataShareByIdQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(GetDataShareByIdQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.Id == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.Id), "Data share ID is required."));
        }

        if (instance.ResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.ResearcherId), "Researcher ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
