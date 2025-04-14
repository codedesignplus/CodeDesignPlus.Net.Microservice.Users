namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "UsersUpdatedDomainEvent")]
public class UsersUpdatedDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : UserBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public static UsersUpdatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, bool isActive)
    {
        return new UsersUpdatedDomainEvent(aggregateId)
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
