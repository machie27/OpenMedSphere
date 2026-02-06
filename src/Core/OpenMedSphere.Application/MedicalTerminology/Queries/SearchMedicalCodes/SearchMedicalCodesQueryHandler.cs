using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Application.Messaging;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Application.MedicalTerminology.Queries.SearchMedicalCodes;

/// <summary>
/// Handles the <see cref="SearchMedicalCodesQuery"/>.
/// </summary>
internal sealed class SearchMedicalCodesQueryHandler(IMedicalTerminologyService terminologyService)
    : IQueryHandler<SearchMedicalCodesQuery, IReadOnlyList<MedicalCodeResponse>>
{
    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<MedicalCodeResponse>>> HandleAsync(
        SearchMedicalCodesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.SearchText))
        {
            return Result<IReadOnlyList<MedicalCodeResponse>>.Failure("Search text cannot be empty.");
        }

        IReadOnlyList<MedicalCode> codes =
            await terminologyService.SearchAsync(query.SearchText, query.CodingSystem, cancellationToken);

        IReadOnlyList<MedicalCodeResponse> responses = codes
            .Select(c => new MedicalCodeResponse
            {
                Code = c.Code,
                DisplayName = c.DisplayName,
                CodingSystem = c.CodingSystem,
                EntityUri = c.EntityUri
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<MedicalCodeResponse>>.Success(responses);
    }
}
