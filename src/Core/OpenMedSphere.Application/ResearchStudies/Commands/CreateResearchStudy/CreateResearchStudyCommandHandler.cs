using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.ResearchStudies.Commands.CreateResearchStudy;

/// <summary>
/// Handles the <see cref="CreateResearchStudyCommand"/>.
/// </summary>
internal sealed class CreateResearchStudyCommandHandler(
    IResearchStudyRepository repository,
    IAnonymizationPolicyRepository policyRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateResearchStudyCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(
        CreateResearchStudyCommand command,
        CancellationToken cancellationToken = default)
    {
        AnonymizationPolicy? policy =
            await policyRepository.GetByIdAsync(command.AnonymizationPolicyId, cancellationToken);

        if (policy is null)
        {
            return Result<Guid>.NotFound(
                $"Anonymization policy with ID '{command.AnonymizationPolicyId}' not found.");
        }

        ResearchStudy? existing = await repository.GetByCodeAsync(command.StudyCode, cancellationToken);
        if (existing is not null)
        {
            return Result<Guid>.Conflict($"A study with code '{command.StudyCode}' already exists.");
        }

        StudyCode studyCode = StudyCode.Create(command.StudyCode);
        DateRange studyPeriod = DateRange.Create(command.StudyPeriodStart, command.StudyPeriodEnd);

        ResearchStudy study = ResearchStudy.Create(
            studyCode,
            command.Title,
            command.PrincipalInvestigator,
            command.Institution,
            studyPeriod,
            command.AnonymizationPolicyId,
            command.Description);

        if (!string.IsNullOrWhiteSpace(command.ResearchArea))
        {
            study.SetResearchArea(command.ResearchArea);
        }

        if (command.MaxParticipants.HasValue)
        {
            study.SetMaxParticipants(command.MaxParticipants.Value);
        }

        await repository.AddAsync(study, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(study.Id);
    }
}
