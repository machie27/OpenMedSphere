using Microsoft.EntityFrameworkCore;
using OpenMedSphere.Application.Abstractions.Data;

namespace OpenMedSphere.Infrastructure.Persistence;

/// <summary>
/// Detects concurrency conflicts using EF Core's <see cref="DbUpdateConcurrencyException"/>.
/// </summary>
internal sealed class EfCoreConcurrencyConflictDetector : IConcurrencyConflictDetector
{
    /// <inheritdoc />
    public bool IsConcurrencyConflict(Exception exception) =>
        exception is DbUpdateConcurrencyException;
}
