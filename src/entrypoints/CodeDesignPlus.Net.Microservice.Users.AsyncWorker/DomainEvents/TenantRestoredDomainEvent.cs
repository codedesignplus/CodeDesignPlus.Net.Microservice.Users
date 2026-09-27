namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;

/// <summary>
/// ms-tenants restauró una copropiedad eliminada antes de que venciera su plazo.
/// </summary>
[EventKey("TenantAggregate", 1, "TenantRestoredDomainEvent", "ms-tenants", "codedesignplus")]
public class TenantRestoredDomainEvent(
    Guid aggregateId,
    string name,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = name;

    public static TenantRestoredDomainEvent Create(Guid aggregateId, string name)
    {
        return new TenantRestoredDomainEvent(aggregateId, name);
    }
}
