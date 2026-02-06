using OpenMedSphere.Application.Abstractions.Specifications;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Application.Specifications;

/// <summary>
/// Specification for searching research studies with multiple criteria.
/// </summary>
public sealed class ResearchStudySearchSpecification : Specification<ResearchStudy>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResearchStudySearchSpecification"/> class.
    /// </summary>
    /// <param name="researchArea">Optional research area to search for.</param>
    /// <param name="titleSearch">Optional title text to search for.</param>
    /// <param name="activeOnly">Optional filter for active status.</param>
    /// <param name="overlapStart">Optional study period overlap start date.</param>
    /// <param name="overlapEnd">Optional study period overlap end date.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    public ResearchStudySearchSpecification(
        string? researchArea = null,
        string? titleSearch = null,
        bool? activeOnly = null,
        DateTime? overlapStart = null,
        DateTime? overlapEnd = null,
        int page = 1,
        int pageSize = 20)
    {
        if (!string.IsNullOrWhiteSpace(researchArea))
        {
            var researchAreaLower = researchArea.ToLower();
            AddFilter(r => r.ResearchArea != null &&
                           r.ResearchArea.ToLower().Contains(researchAreaLower));
        }

        if (!string.IsNullOrWhiteSpace(titleSearch))
        {
            var titleLower = titleSearch.ToLower();
            AddFilter(r => r.Title.ToLower().Contains(titleLower));
        }

        if (activeOnly.HasValue)
        {
            AddFilter(r => r.IsActive == activeOnly.Value);
        }

        if (overlapStart.HasValue && overlapEnd.HasValue)
        {
            AddFilter(r => r.StudyPeriod.Start <= overlapEnd.Value &&
                           r.StudyPeriod.End >= overlapStart.Value);
        }

        AddOrderByDescending(r => r.CreatedAtUtc);
        ApplyPaging((page - 1) * pageSize, pageSize);
    }
}
