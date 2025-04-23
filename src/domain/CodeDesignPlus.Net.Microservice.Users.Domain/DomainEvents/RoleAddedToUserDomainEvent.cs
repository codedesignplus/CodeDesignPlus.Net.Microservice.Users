namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "RoleAddedToUserDomainEvent")]
public class RoleAddedToUserDomainEvent(
     Guid aggregateId,
     string? displayName,
     string role,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;
    public string Role { get; } = role;
    public static RoleAddedToUserDomainEvent Create(Guid aggregateId, string? displayName, string role)
    {
        return new RoleAddedToUserDomainEvent(aggregateId, displayName, role);
    }
}
