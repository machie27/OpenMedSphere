namespace OpenMedSphere.Application.Common;

/// <summary>
/// Utility for escaping LIKE wildcard characters in user input.
/// Prevents unintended pattern matching in PostgreSQL LIKE/ILIKE queries.
/// Values are parameterized by EF Core, preventing SQL injection.
/// </summary>
public static class LikePatternHelper
{
    /// <summary>
    /// Escapes LIKE wildcard characters (<c>%</c>, <c>_</c>, <c>\</c>) in user input.
    /// Relies on PostgreSQL's default backslash (<c>\</c>) as the LIKE escape character.
    /// </summary>
    /// <param name="input">The raw user input to escape.</param>
    /// <returns>The escaped string safe for use in LIKE patterns.</returns>
    public static string EscapeLikeWildcards(string input) =>
        input
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
}
