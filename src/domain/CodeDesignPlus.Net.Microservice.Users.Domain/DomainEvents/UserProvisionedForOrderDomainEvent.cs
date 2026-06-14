namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "UserProvisionedForOrderDomainEvent")]
public class UserProvisionedForOrderDomainEvent(
    Guid aggregateId,
    Guid orderId,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Guid OrderId { get; } = orderId;

    public static UserProvisionedForOrderDomainEvent Create(Guid aggregateId, Guid orderId)
        => new(aggregateId, orderId);
}
