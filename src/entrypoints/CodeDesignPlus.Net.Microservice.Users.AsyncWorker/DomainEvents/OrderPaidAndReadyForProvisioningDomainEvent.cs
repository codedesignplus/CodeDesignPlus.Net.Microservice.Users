using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Dtos;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;

[EventKey("OrderAggregate", 1, "OrderPaidAndReadyForProvisioningDomainEvent", "ms-licenses")]
public class OrderPaidAndReadyForProvisioningDomainEvent(
    Guid aggregateId,
    Tenant tenantDetail,
    Guid buyerId,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Tenant TenantDetail { get; } = tenantDetail;
    public Guid BuyerId { get; } = buyerId;

    public static OrderPaidAndReadyForProvisioningDomainEvent Create(Guid aggregateId, Tenant tenantDetail, Guid buyerId)
    {
        return new OrderPaidAndReadyForProvisioningDomainEvent(aggregateId, tenantDetail, buyerId);
    }
}
