using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

namespace OpenMedSphere.Application.PatientData.Queries.GetPatientDataById;

/// <summary>
/// Handles the <see cref="GetPatientDataByIdQuery"/>.
/// </summary>
internal sealed class GetPatientDataByIdQueryHandler(IPatientDataRepository repository)
    : IQueryHandler<GetPatientDataByIdQuery, PatientDataResponse>
{
    /// <inheritdoc />
    public async Task<Result<PatientDataResponse>> HandleAsync(
        GetPatientDataByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.PatientData? patientData =
            await repository.GetByIdAsync(query.Id, cancellationToken);

        if (patientData is null)
        {
            return Result<PatientDataResponse>.Failure($"Patient data with ID '{query.Id}' not found.");
        }

        PatientDataResponse response = new()
        {
            Id = patientData.Id,
            PatientIdentifier = patientData.PatientId.Value,
            YearOfBirth = patientData.YearOfBirth,
            Gender = patientData.Gender,
            Region = patientData.Region,
            PrimaryDiagnosis = patientData.PrimaryDiagnosis,
            PrimaryDiagnosisIcdCode = patientData.PrimaryDiagnosisCode?.Code,
            SecondaryDiagnoses = patientData.SecondaryDiagnoses.AsReadOnly(),
            Medications = patientData.Medications.AsReadOnly(),
            IsAnonymized = patientData.IsAnonymized,
            CollectedAtUtc = patientData.CollectedAtUtc,
            CreatedAtUtc = patientData.CreatedAtUtc
        };

        return Result<PatientDataResponse>.Success(response);
    }
}
