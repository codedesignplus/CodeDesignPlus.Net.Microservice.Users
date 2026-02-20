namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "UserPictureUpdatedDomainEvent", autoCreate: false)]
public class UserPictureUpdatedDomainEvent(
     Guid aggregateId,
     string Name,
     string Target,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = Name;
    public string Target { get; } = Target;

    public static UserPictureUpdatedDomainEvent Create(Guid aggregateId, string name, string target)
    {
        return new UserPictureUpdatedDomainEvent(aggregateId, name, target);
    }
}
