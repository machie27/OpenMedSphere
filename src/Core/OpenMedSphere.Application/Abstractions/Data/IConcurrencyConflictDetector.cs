namespace OpenMedSphere.Application.Abstractions.Data;

/// <summary>
/// Detects whether an exception represents a concurrency conflict (optimistic concurrency violation).
/// Abstracts database-specific exception inspection so the Application layer
/// does not depend on EF Core types.
/// </summary>
public interface IConcurrencyConflictDetector
{
    /// <summary>
    /// Determines whether the exception represents a concurrency conflict.
    /// </summary>
    /// <param name="exception">The exception to inspect.</param>
    /// <returns><see langword="true"/> if the exception is a concurrency conflict.</returns>
    bool IsConcurrencyConflict(Exception exception);
}
