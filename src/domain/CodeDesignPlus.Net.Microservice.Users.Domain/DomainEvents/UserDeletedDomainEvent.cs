namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "UserDeletedDomainEvent")]
public class UserDeletedDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : UserBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public static UserDeletedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, bool isActive)
    {
        return new UserDeletedDomainEvent(aggregateId)
        {
            FirtName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DisplayName = displayName,
            IsActive = isActive
        };
    }
}
