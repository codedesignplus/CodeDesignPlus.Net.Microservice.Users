namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;

/// <summary>
/// ms-tenants dio de baja una copropiedad: se puede restaurar hasta <see cref="PurgeAfter"/>.
/// </summary>
/// <remarks>
/// Solo se declara lo que se usa. <see cref="PurgeAfter"/> es opcional porque las versiones de ms-tenants anteriores a
/// la baja diferida borraban en el acto y no lo enviaban.
/// </remarks>
[EventKey("TenantAggregate", 1, "TenantDeletedDomainEvent", "ms-tenants", "codedesignplus")]
public class TenantDeletedDomainEvent(
    Guid aggregateId,
    string name,
    Instant? purgeAfter,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = name;

    public Instant? PurgeAfter { get; } = purgeAfter;

    public static TenantDeletedDomainEvent Create(Guid aggregateId, string name, Instant? purgeAfter)
    {
        return new TenantDeletedDomainEvent(aggregateId, name, purgeAfter);
    }
}
