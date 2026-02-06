using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.PatientData.Commands.CreatePatientData;

/// <summary>
/// Handles the <see cref="CreatePatientDataCommand"/>.
/// </summary>
internal sealed class CreatePatientDataCommandHandler(
    IPatientDataRepository repository,
    IUnitOfWork unitOfWork,
    IMedicalTerminologyService terminologyService)
    : ICommandHandler<CreatePatientDataCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Result<Guid>> HandleAsync(
        CreatePatientDataCommand command,
        CancellationToken cancellationToken = default)
    {
        PatientIdentifier patientId = PatientIdentifier.Generate();

        Domain.Entities.PatientData patientData = Domain.Entities.PatientData.Create(patientId);

        if (command.YearOfBirth.HasValue || command.Gender is not null || command.Region is not null)
        {
            patientData.UpdateDemographics(command.YearOfBirth, command.Gender, command.Region);
        }

        if (!string.IsNullOrWhiteSpace(command.PrimaryDiagnosis))
        {
            patientData.SetPrimaryDiagnosis(command.PrimaryDiagnosis);
        }

        if (!string.IsNullOrWhiteSpace(command.PrimaryDiagnosisIcdCode))
        {
            MedicalCode? code = await terminologyService.GetByCodeAsync(
                command.PrimaryDiagnosisIcdCode, cancellationToken: cancellationToken);

            if (code is null)
            {
                return Result<Guid>.Failure(
                    $"ICD code '{command.PrimaryDiagnosisIcdCode}' was not found in any configured coding system.");
            }

            patientData.SetPrimaryDiagnosisCode(code);
        }

        if (command.SecondaryDiagnoses is not null)
        {
            foreach (string diagnosis in command.SecondaryDiagnoses)
            {
                patientData.AddSecondaryDiagnosis(diagnosis);
            }
        }

        if (command.Medications is not null)
        {
            foreach (string medication in command.Medications)
            {
                patientData.AddMedication(medication);
            }
        }

        if (!string.IsNullOrWhiteSpace(command.ClinicalNotes))
        {
            patientData.UpdateClinicalNotes(command.ClinicalNotes);
        }

        await repository.AddAsync(patientData, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(patientData.Id);
    }
}
