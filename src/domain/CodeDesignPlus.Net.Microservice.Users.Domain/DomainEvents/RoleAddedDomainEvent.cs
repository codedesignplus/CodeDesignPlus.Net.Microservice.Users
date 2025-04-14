namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "RoleAddedDomainEvent")]
public class RoleAddedDomainEvent(
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
    public static RoleAddedDomainEvent Create(Guid aggregateId, string? displayName, string role)
    {
        return new RoleAddedDomainEvent(aggregateId, displayName, role);
    }
}
