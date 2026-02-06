using OpenMedSphere.Application.Abstractions.Specifications;

namespace OpenMedSphere.Application.Specifications;

/// <summary>
/// Specification for searching patient data with multiple criteria.
/// </summary>
public sealed class PatientDataSearchSpecification : Specification<Domain.Entities.PatientData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatientDataSearchSpecification"/> class.
    /// </summary>
    /// <param name="diagnosisText">Optional diagnosis text to search for.</param>
    /// <param name="icdCode">Optional ICD code to filter by.</param>
    /// <param name="region">Optional region to filter by.</param>
    /// <param name="anonymizedOnly">Optional filter for anonymized status.</param>
    /// <param name="collectedAfter">Optional filter for collection date.</param>
    /// <param name="collectedBefore">Optional filter for collection date.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    public PatientDataSearchSpecification(
        string? diagnosisText = null,
        string? icdCode = null,
        string? region = null,
        bool? anonymizedOnly = null,
        DateTime? collectedAfter = null,
        DateTime? collectedBefore = null,
        int page = 1,
        int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        if (!string.IsNullOrWhiteSpace(diagnosisText))
        {
            var diagnosisTextLower = EscapeLikeWildcards(diagnosisText.ToLower());
            AddFilter(p =>
                (p.PrimaryDiagnosis != null && p.PrimaryDiagnosis.ToLower().Contains(diagnosisTextLower)) ||
                p.SecondaryDiagnoses.Any(d => d.ToLower().Contains(diagnosisTextLower)));
        }

        if (!string.IsNullOrWhiteSpace(icdCode))
        {
            AddFilter(p =>
                (p.PrimaryDiagnosisCode != null && p.PrimaryDiagnosisCode.Code == icdCode) ||
                p.SecondaryDiagnosisCodes.Any(c => c.Code == icdCode));
        }

        if (!string.IsNullOrWhiteSpace(region))
        {
            var regionLower = EscapeLikeWildcards(region.ToLower());
            AddFilter(p => p.Region != null && p.Region.ToLower().Contains(regionLower));
        }

        if (anonymizedOnly.HasValue)
        {
            AddFilter(p => p.IsAnonymized == anonymizedOnly.Value);
        }

        if (collectedAfter.HasValue)
        {
            AddFilter(p => p.CollectedAtUtc >= collectedAfter.Value);
        }

        if (collectedBefore.HasValue)
        {
            AddFilter(p => p.CollectedAtUtc <= collectedBefore.Value);
        }

        AddOrderByDescending(p => p.CreatedAtUtc);
        ApplyPaging((page - 1) * pageSize, pageSize);
    }

    private static string EscapeLikeWildcards(string input) =>
        input
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}
