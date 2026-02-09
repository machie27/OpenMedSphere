namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Shared validation constants used across validators.
/// </summary>
internal static class ValidationConstants
{
    public const int MaxGenderLength = 50;
    public const int MaxRegionLength = 200;
    public const int MaxDiagnosisLength = 500;
    public const int MaxIcdCodeLength = 50;
    public const int MaxNotesLength = 10000;
    public const int MaxSecondaryDiagnoses = 50;
    public const int MaxMedications = 100;
    public const int MinYearOfBirth = 1900;
    public const int MaxStudyCodeLength = 50;
    public const int MaxTitleLength = 500;
    public const int MaxInvestigatorLength = 200;
    public const int MaxInstitutionLength = 300;
    public const int MaxDescriptionLength = 5000;
    public const int MaxResearchAreaLength = 200;
    public const int MaxSearchTextLength = 200;
    public const int MinPage = 1;
    public const int MaxPageSize = 100;
}
