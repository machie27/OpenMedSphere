using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OpenMedSphere.Domain.Entities;

namespace OpenMedSphere.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that creates audit log entries for tracked entity changes.
/// </summary>
internal sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly HashSet<Type> AuditedTypes =
    [
        typeof(PatientData),
        typeof(ResearchStudy),
        typeof(AnonymizationPolicy),
        typeof(Researcher),
        typeof(DataShare)
    ];

    private static readonly HashSet<string> ExcludedFromAudit =
    [
        nameof(DataShare.EncryptedPayload),
        nameof(DataShare.EncapsulatedKey),
        nameof(DataShare.Signature)
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void AddAuditEntries(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        List<AuditLogEntry> auditEntries = [];

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (!AuditedTypes.Contains(entry.Entity.GetType()))
            {
                continue;
            }

            if (entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            string entityType = entry.Entity.GetType().Name;

            string entityId;
            try
            {
                entityId = entry.Property("Id").CurrentValue?.ToString() ?? string.Empty;
            }
            catch (InvalidOperationException)
            {
                entityId = string.Empty;
            }

            string? action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Modified",
                EntityState.Deleted => "Deleted",
                _ => null
            };

            if (action is null)
            {
                continue;
            }

            string? oldValues = entry.State is EntityState.Modified or EntityState.Deleted
                ? SerializeValues(entry, useOriginalValues: true)
                : null;

            string? newValues = entry.State is EntityState.Added or EntityState.Modified
                ? SerializeValues(entry, useOriginalValues: false)
                : null;

            AuditLogEntry auditEntry = AuditLogEntry.Create(
                entityType, entityId, action, oldValues, newValues, userId: null);

            auditEntries.Add(auditEntry);
        }

        if (auditEntries.Count > 0)
        {
            context.Set<AuditLogEntry>().AddRange(auditEntries);
        }
    }

    private static string SerializeValues(EntityEntry entry, bool useOriginalValues)
    {
        Dictionary<string, object?> values = [];

        foreach (PropertyEntry property in entry.Properties)
        {
            if (property.Metadata.IsPrimaryKey())
            {
                continue;
            }

            string propertyName = property.Metadata.Name;

            if (entry.Entity is DataShare && ExcludedFromAudit.Contains(propertyName))
            {
                continue;
            }
            object? value = useOriginalValues ? property.OriginalValue : property.CurrentValue;
            values[propertyName] = value;
        }

        return JsonSerializer.Serialize(values, JsonOptions);
    }
}
