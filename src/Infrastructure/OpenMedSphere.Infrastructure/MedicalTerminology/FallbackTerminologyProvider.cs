using Microsoft.Extensions.Logging;
using OpenMedSphere.Application.Abstractions.MedicalTerminology;
using OpenMedSphere.Domain.ValueObjects;

namespace OpenMedSphere.Infrastructure.MedicalTerminology;

/// <summary>
/// Fallback ICD-11 medical terminology provider with a static dataset of common codes.
/// Used when no ICD-11 API credentials are configured, ensuring basic lookup functionality
/// is always available.
/// </summary>
internal sealed partial class FallbackTerminologyProvider(
    ILogger<FallbackTerminologyProvider> logger) : IMedicalTerminologyProvider
{
    private static readonly IReadOnlyList<MedicalCode> CommonCodes =
    [
        MedicalCode.Create("BA00", "Essential hypertension", "ICD-11"),
        MedicalCode.Create("BA01", "Hypertensive heart disease", "ICD-11"),
        MedicalCode.Create("BA02", "Hypertensive renal disease", "ICD-11"),
        MedicalCode.Create("BA80", "Hypotension", "ICD-11"),
        MedicalCode.Create("5A11", "Type 2 diabetes mellitus", "ICD-11"),
        MedicalCode.Create("5A10", "Type 1 diabetes mellitus", "ICD-11"),
        MedicalCode.Create("5A14", "Gestational diabetes mellitus", "ICD-11"),
        MedicalCode.Create("CA40", "Asthma", "ICD-11"),
        MedicalCode.Create("CA20", "Pneumonia", "ICD-11"),
        MedicalCode.Create("CA22", "Bronchitis", "ICD-11"),
        MedicalCode.Create("CA07", "Chronic obstructive pulmonary disease", "ICD-11"),
        MedicalCode.Create("2A00", "Malignant neoplasms of breast", "ICD-11"),
        MedicalCode.Create("2B90", "Malignant neoplasms of colon", "ICD-11"),
        MedicalCode.Create("2C25", "Malignant neoplasms of lung", "ICD-11"),
        MedicalCode.Create("2D10", "Malignant neoplasms of prostate", "ICD-11"),
        MedicalCode.Create("8B11", "Stroke, not specified as haemorrhage or infarction", "ICD-11"),
        MedicalCode.Create("BA80.0", "Orthostatic hypotension", "ICD-11"),
        MedicalCode.Create("BA41", "Coronary artery disease", "ICD-11"),
        MedicalCode.Create("BA60", "Heart failure", "ICD-11"),
        MedicalCode.Create("BA42", "Angina pectoris", "ICD-11"),
        MedicalCode.Create("DA01", "Gastro-oesophageal reflux disease", "ICD-11"),
        MedicalCode.Create("DA24", "Crohn disease", "ICD-11"),
        MedicalCode.Create("DA25", "Ulcerative colitis", "ICD-11"),
        MedicalCode.Create("DA90", "Irritable bowel syndrome", "ICD-11"),
        MedicalCode.Create("FA01", "Rheumatoid arthritis", "ICD-11"),
        MedicalCode.Create("FA20", "Osteoarthritis", "ICD-11"),
        MedicalCode.Create("FA24", "Gout", "ICD-11"),
        MedicalCode.Create("FB40", "Osteoporosis", "ICD-11"),
        MedicalCode.Create("6A00", "Depressive episode", "ICD-11"),
        MedicalCode.Create("6A01", "Recurrent depressive disorder", "ICD-11"),
        MedicalCode.Create("6A20", "Generalised anxiety disorder", "ICD-11"),
        MedicalCode.Create("6A40", "Obsessive-compulsive disorder", "ICD-11"),
        MedicalCode.Create("6A70", "Post traumatic stress disorder", "ICD-11"),
        MedicalCode.Create("6D10", "Alzheimer disease", "ICD-11"),
        MedicalCode.Create("6D80", "Parkinson disease", "ICD-11"),
        MedicalCode.Create("8A00", "Epilepsy", "ICD-11"),
        MedicalCode.Create("8A80", "Migraine", "ICD-11"),
        MedicalCode.Create("EA10", "Atopic dermatitis", "ICD-11"),
        MedicalCode.Create("EA90", "Psoriasis", "ICD-11"),
        MedicalCode.Create("1A07", "Influenza due to identified seasonal influenza virus", "ICD-11"),
        MedicalCode.Create("1C62", "COVID-19", "ICD-11"),
        MedicalCode.Create("1A00", "Cholera", "ICD-11"),
        MedicalCode.Create("1F20", "Malaria", "ICD-11"),
        MedicalCode.Create("1B10", "Tuberculosis", "ICD-11"),
        MedicalCode.Create("GB61", "Chronic kidney disease", "ICD-11"),
        MedicalCode.Create("GB90", "Urinary tract infection", "ICD-11"),
        MedicalCode.Create("DB94", "Liver cirrhosis", "ICD-11"),
        MedicalCode.Create("5B55", "Obesity", "ICD-11"),
        MedicalCode.Create("5A80", "Hypothyroidism", "ICD-11"),
        MedicalCode.Create("5A00", "Hyperthyroidism", "ICD-11")
    ];

    /// <inheritdoc />
    public string CodingSystem => "ICD-11";

    /// <inheritdoc />
    public Task<IReadOnlyList<MedicalCode>> SearchAsync(
        string searchText,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);

        IReadOnlyList<MedicalCode> results = CommonCodes
            .Where(c => c.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        c.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

        LogSearchCompleted(searchText, results.Count);

        return Task.FromResult(results);
    }

    /// <inheritdoc />
    public Task<MedicalCode?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        MedicalCode? result = CommonCodes
            .FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        LogCodeLookupCompleted(code, result is not null);

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<MedicalCode?> GetByEntityUriAsync(
        string entityUri,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityUri);

        MedicalCode? result = CommonCodes
            .FirstOrDefault(c => c.EntityUri is not null &&
                                 c.EntityUri.Equals(entityUri, StringComparison.OrdinalIgnoreCase));

        LogEntityUriLookupCompleted(entityUri, result is not null);

        return Task.FromResult(result);
    }

    [LoggerMessage(EventId = 2040, Level = LogLevel.Debug, Message = "Fallback search for '{SearchText}' returned {ResultCount} results")]
    private partial void LogSearchCompleted(string searchText, int resultCount);

    [LoggerMessage(EventId = 2041, Level = LogLevel.Debug, Message = "Fallback code lookup for '{Code}', found: {Found}")]
    private partial void LogCodeLookupCompleted(string code, bool found);

    [LoggerMessage(EventId = 2042, Level = LogLevel.Debug, Message = "Fallback entity URI lookup for '{EntityUri}', found: {Found}")]
    private partial void LogEntityUriLookupCompleted(string entityUri, bool found);

    [LoggerMessage(EventId = 2043, Level = LogLevel.Debug, Message = "Fallback provider initialized with {CodeCount} static codes")]
    private partial void LogProviderInitialized(int codeCount);
}
