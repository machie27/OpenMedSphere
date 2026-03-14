using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Queries.GetOutgoingShares;

/// <summary>
/// Validates the <see cref="GetOutgoingSharesQuery"/>.
/// </summary>
internal sealed class GetOutgoingSharesQueryValidator : IValidator<GetOutgoingSharesQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(GetOutgoingSharesQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.ResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.ResearcherId), "Researcher ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
