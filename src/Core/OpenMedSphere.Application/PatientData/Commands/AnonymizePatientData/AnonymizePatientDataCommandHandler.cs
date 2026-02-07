using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.PatientData.Commands.AnonymizePatientData;

/// <summary>
/// Handles the <see cref="AnonymizePatientDataCommand"/>.
/// </summary>
internal sealed class AnonymizePatientDataCommandHandler(
    IPatientDataRepository patientDataRepository,
    IAnonymizationPolicyRepository policyRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AnonymizePatientDataCommand>
{
    /// <inheritdoc />
    public async Task<Result> HandleAsync(
        AnonymizePatientDataCommand command,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.PatientData? patientData =
            await patientDataRepository.GetByIdAsync(command.PatientDataId, cancellationToken);

        if (patientData is null)
        {
            return Result.NotFound($"Patient data with ID '{command.PatientDataId}' not found.");
        }

        if (patientData.IsAnonymized)
        {
            return Result.InvalidOperation("Patient data is already anonymized.");
        }

        AnonymizationPolicy? policy =
            await policyRepository.GetByIdAsync(command.PolicyId, cancellationToken);

        if (policy is null)
        {
            return Result.NotFound($"Anonymization policy with ID '{command.PolicyId}' not found.");
        }

        if (!policy.IsActive)
        {
            return Result.InvalidOperation("Cannot use an inactive anonymization policy.");
        }

        patientData.MarkAsAnonymized(command.PolicyId);

        patientDataRepository.Update(patientData);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
