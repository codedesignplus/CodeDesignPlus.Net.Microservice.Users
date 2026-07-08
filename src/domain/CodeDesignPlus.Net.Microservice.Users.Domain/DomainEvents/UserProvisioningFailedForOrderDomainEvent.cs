namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "UserProvisioningFailedForOrderDomainEvent")]
public class UserProvisioningFailedForOrderDomainEvent(
    Guid aggregateId,
    Guid orderId,
    string reason,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Guid OrderId { get; } = orderId;
    public string Reason { get; } = reason;

    public static UserProvisioningFailedForOrderDomainEvent Create(Guid userId, Guid orderId, string reason)
        => new(userId, orderId, reason);
}
