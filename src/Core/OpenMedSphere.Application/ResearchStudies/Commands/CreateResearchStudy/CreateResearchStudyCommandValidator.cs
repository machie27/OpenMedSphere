using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.ResearchStudies.Commands.CreateResearchStudy;

/// <summary>
/// Validates the <see cref="CreateResearchStudyCommand"/>.
/// </summary>
internal sealed class CreateResearchStudyCommandValidator : IValidator<CreateResearchStudyCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(CreateResearchStudyCommand instance, CancellationToken cancellationToken = default)
    {
        List<ValidationError> errors = [];

        if (string.IsNullOrWhiteSpace(instance.StudyCode))
        {
            errors.Add(new ValidationError(nameof(instance.StudyCode), "Study code is required."));
        }
        else if (instance.StudyCode.Length > ValidationConstants.MaxStudyCodeLength)
        {
            errors.Add(new ValidationError(nameof(instance.StudyCode), $"Study code must not exceed {ValidationConstants.MaxStudyCodeLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Title))
        {
            errors.Add(new ValidationError(nameof(instance.Title), "Title is required."));
        }
        else if (instance.Title.Length > ValidationConstants.MaxTitleLength)
        {
            errors.Add(new ValidationError(nameof(instance.Title), $"Title must not exceed {ValidationConstants.MaxTitleLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.PrincipalInvestigator))
        {
            errors.Add(new ValidationError(nameof(instance.PrincipalInvestigator), "Principal investigator is required."));
        }
        else if (instance.PrincipalInvestigator.Length > ValidationConstants.MaxInvestigatorLength)
        {
            errors.Add(new ValidationError(nameof(instance.PrincipalInvestigator), $"Principal investigator must not exceed {ValidationConstants.MaxInvestigatorLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Institution))
        {
            errors.Add(new ValidationError(nameof(instance.Institution), "Institution is required."));
        }
        else if (instance.Institution.Length > ValidationConstants.MaxInstitutionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Institution), $"Institution must not exceed {ValidationConstants.MaxInstitutionLength} characters."));
        }

        if (instance.Description is not null && instance.Description.Length > ValidationConstants.MaxDescriptionLength)
        {
            errors.Add(new ValidationError(nameof(instance.Description), $"Description must not exceed {ValidationConstants.MaxDescriptionLength} characters."));
        }

        if (instance.StudyPeriodEnd <= instance.StudyPeriodStart)
        {
            errors.Add(new ValidationError(nameof(instance.StudyPeriodEnd), "Study period end must be after start."));
        }

        if (instance.AnonymizationPolicyId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(instance.AnonymizationPolicyId), "Anonymization policy ID is required."));
        }

        if (instance.MaxParticipants.HasValue && instance.MaxParticipants.Value <= 0)
        {
            errors.Add(new ValidationError(nameof(instance.MaxParticipants), "Max participants must be greater than 0."));
        }

        if (instance.ResearchArea is not null && instance.ResearchArea.Length > ValidationConstants.MaxResearchAreaLength)
        {
            errors.Add(new ValidationError(nameof(instance.ResearchArea), $"Research area must not exceed {ValidationConstants.MaxResearchAreaLength} characters."));
        }

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors });
    }
}
