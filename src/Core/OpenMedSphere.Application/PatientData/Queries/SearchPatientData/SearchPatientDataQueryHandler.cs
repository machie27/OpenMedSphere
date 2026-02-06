using OpenMedSphere.Application.Abstractions.Data;
using OpenMedSphere.Application.Common;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Application.Specifications;

namespace OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

/// <summary>
/// Handles the <see cref="SearchPatientDataQuery"/>.
/// </summary>
internal sealed class SearchPatientDataQueryHandler(IPatientDataRepository repository)
    : IQueryHandler<SearchPatientDataQuery, PagedResult<PatientDataResponse>>
{
    /// <inheritdoc />
    public async Task<Result<PagedResult<PatientDataResponse>>> HandleAsync(
        SearchPatientDataQuery query,
        CancellationToken cancellationToken = default)
    {
        PatientDataSearchSpecification specification = new(
            diagnosisText: query.DiagnosisText,
            icdCode: query.IcdCode,
            region: query.Region,
            anonymizedOnly: query.AnonymizedOnly,
            collectedAfter: query.CollectedAfter,
            collectedBefore: query.CollectedBefore,
            page: query.Page,
            pageSize: query.PageSize);

        IReadOnlyList<Domain.Entities.PatientData> items =
            await repository.FindAsync(specification, cancellationToken);

        int totalCount = await repository.CountAsync(specification, cancellationToken);

        IReadOnlyList<PatientDataResponse> responses = items
            .Select(p => new PatientDataResponse
            {
                Id = p.Id,
                PatientIdentifier = p.PatientId.Value,
                YearOfBirth = p.YearOfBirth,
                Gender = p.Gender,
                Region = p.Region,
                PrimaryDiagnosis = p.PrimaryDiagnosis,
                PrimaryDiagnosisIcdCode = p.PrimaryDiagnosisCode?.Code,
                SecondaryDiagnoses = p.SecondaryDiagnoses.ToList(),
                Medications = p.Medications.ToList(),
                IsAnonymized = p.IsAnonymized,
                CollectedAtUtc = p.CollectedAtUtc,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToList()
            .AsReadOnly();

        PagedResult<PatientDataResponse> result = new()
        {
            Items = responses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        return Result<PagedResult<PatientDataResponse>>.Success(result);
    }
}
