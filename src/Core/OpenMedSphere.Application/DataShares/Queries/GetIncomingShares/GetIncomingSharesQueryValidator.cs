using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Queries.GetIncomingShares;

/// <summary>
/// Validates the <see cref="GetIncomingSharesQuery"/>.
/// </summary>
internal sealed class GetIncomingSharesQueryValidator : IValidator<GetIncomingSharesQuery>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(GetIncomingSharesQuery instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.ResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.ResearcherId), "Researcher ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
