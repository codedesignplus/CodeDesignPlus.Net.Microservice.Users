namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "RoleRemovedToUser")]
public class RoleRemovedToUserDomainEvent(
     Guid aggregateId,
     string displayName,
     string role,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string DisplayName { get; } = displayName;
    public string Role { get; } = role;
    public static RoleRemovedToUserDomainEvent Create(Guid aggregateId, string displayName, string role)
    {
        return new RoleRemovedToUserDomainEvent(aggregateId, displayName, role);
    }
}
