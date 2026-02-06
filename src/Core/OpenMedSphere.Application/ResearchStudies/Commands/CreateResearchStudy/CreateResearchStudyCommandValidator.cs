using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.ResearchStudies.Commands.CreateResearchStudy;

/// <summary>
/// Validates the <see cref="CreateResearchStudyCommand"/>.
/// </summary>
internal sealed class CreateResearchStudyCommandValidator : IValidator<CreateResearchStudyCommand>
{
    /// <inheritdoc />
    public ValidationResult Validate(CreateResearchStudyCommand instance)
    {
        List<ValidationError> errors = [];

        if (string.IsNullOrWhiteSpace(instance.StudyCode))
        {
            errors.Add(new ValidationError(nameof(instance.StudyCode), "Study code is required."));
        }
        else if (instance.StudyCode.Length > 50)
        {
            errors.Add(new ValidationError(nameof(instance.StudyCode), "Study code must not exceed 50 characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Title))
        {
            errors.Add(new ValidationError(nameof(instance.Title), "Title is required."));
        }
        else if (instance.Title.Length > 500)
        {
            errors.Add(new ValidationError(nameof(instance.Title), "Title must not exceed 500 characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.PrincipalInvestigator))
        {
            errors.Add(new ValidationError(nameof(instance.PrincipalInvestigator), "Principal investigator is required."));
        }
        else if (instance.PrincipalInvestigator.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.PrincipalInvestigator), "Principal investigator must not exceed 200 characters."));
        }

        if (string.IsNullOrWhiteSpace(instance.Institution))
        {
            errors.Add(new ValidationError(nameof(instance.Institution), "Institution is required."));
        }
        else if (instance.Institution.Length > 300)
        {
            errors.Add(new ValidationError(nameof(instance.Institution), "Institution must not exceed 300 characters."));
        }

        if (instance.Description is not null && instance.Description.Length > 5000)
        {
            errors.Add(new ValidationError(nameof(instance.Description), "Description must not exceed 5000 characters."));
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

        if (instance.ResearchArea is not null && instance.ResearchArea.Length > 200)
        {
            errors.Add(new ValidationError(nameof(instance.ResearchArea), "Research area must not exceed 200 characters."));
        }

        return errors.Count == 0 ? ValidationResult.Success() : new ValidationResult { Errors = errors };
    }
}
