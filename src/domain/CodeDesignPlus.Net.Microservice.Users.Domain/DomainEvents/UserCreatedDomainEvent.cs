using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "UserCreatedDomainEvent")]
public class UserCreatedDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : UserBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public static UserCreatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string displayName, bool isActive)
    {
        return new UserCreatedDomainEvent(aggregateId)
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
