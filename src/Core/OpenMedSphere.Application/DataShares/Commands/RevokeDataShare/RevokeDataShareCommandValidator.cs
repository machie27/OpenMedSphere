using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.DataShares.Commands.RevokeDataShare;

/// <summary>
/// Validates the <see cref="RevokeDataShareCommand"/>.
/// </summary>
internal sealed class RevokeDataShareCommandValidator : IValidator<RevokeDataShareCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(RevokeDataShareCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (instance.DataShareId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.DataShareId), "Data share ID is required."));
        }

        if (instance.ResearcherId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.ResearcherId), "Researcher ID is required."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
