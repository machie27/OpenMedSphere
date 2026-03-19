using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenMedSphere.Application.Abstractions.Data;

namespace OpenMedSphere.Infrastructure.Persistence;

/// <summary>
/// Detects unique constraint violations using Npgsql's <see cref="PostgresException"/>
/// and PostgreSQL error code 23505 (unique_violation).
/// </summary>
internal sealed class NpgsqlUniqueConstraintViolationDetector : IUniqueConstraintViolationDetector
{
    /// <inheritdoc />
    public bool IsUniqueConstraintViolation(Exception exception, string indexName)
    {
        return exception is DbUpdateException { InnerException: PostgresException postgresException }
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
            && postgresException.ConstraintName == indexName;
    }
}
