using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "TenantRemovedDomainEvent")]
public class TenantRemovedDomainEvent(
     Guid aggregateId,
     string? displayName,
     TenantEntity tenant,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;
    public TenantEntity Tenant { get; } = tenant;
    public static TenantRemovedDomainEvent Create(Guid aggregateId,  string? displayName, TenantEntity tenant)
    {
        return new TenantRemovedDomainEvent(aggregateId, displayName, tenant);
    }
}
