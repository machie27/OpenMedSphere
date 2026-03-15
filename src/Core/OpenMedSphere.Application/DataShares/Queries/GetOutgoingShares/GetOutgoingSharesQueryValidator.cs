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
