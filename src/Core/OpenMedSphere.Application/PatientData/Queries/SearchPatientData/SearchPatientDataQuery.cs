using OpenMedSphere.Application.Common;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application.PatientData.Queries.SearchPatientData;

/// <summary>
/// Query to search patient data with multiple criteria.
/// </summary>
public sealed record SearchPatientDataQuery : IQuery<PagedResult<PatientDataResponse>>
{
    /// <summary>
    /// Gets the diagnosis text to search for.
    /// </summary>
    public string? DiagnosisText { get; init; }

    /// <summary>
    /// Gets the ICD code to filter by.
    /// </summary>
    public string? IcdCode { get; init; }

    /// <summary>
    /// Gets the region to filter by.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Gets whether to filter by anonymized status.
    /// </summary>
    public bool? AnonymizedOnly { get; init; }

    /// <summary>
    /// Gets the minimum collection date.
    /// </summary>
    public DateTime? CollectedAfter { get; init; }

    /// <summary>
    /// Gets the maximum collection date.
    /// </summary>
    public DateTime? CollectedBefore { get; init; }

    /// <summary>
    /// Gets the page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}
