namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Detects whether an exception represents a unique constraint violation for a given index.
/// Abstracts database-specific exception inspection so the Application layer
/// does not depend on EF Core or Npgsql types.
/// </summary>
public interface IUniqueConstraintViolationDetector
{
    /// <summary>
    /// Determines whether the exception represents a unique constraint violation
    /// on the specified index.
    /// </summary>
    /// <param name="exception">The exception to inspect.</param>
    /// <param name="indexName">The database index name to match against.</param>
    /// <returns><see langword="true"/> if the exception is a unique constraint violation for the index.</returns>
    bool IsUniqueConstraintViolation(Exception exception, string indexName);
}
