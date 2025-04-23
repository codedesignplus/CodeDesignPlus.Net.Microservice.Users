using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "TenantAddedDomainEvent")]
public class TenantAddedDomainEvent(
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

    public static TenantAddedDomainEvent Create(Guid aggregateId, string? displayName, TenantEntity tenant)
    {
        return new TenantAddedDomainEvent(aggregateId, displayName, tenant);
    }
}
