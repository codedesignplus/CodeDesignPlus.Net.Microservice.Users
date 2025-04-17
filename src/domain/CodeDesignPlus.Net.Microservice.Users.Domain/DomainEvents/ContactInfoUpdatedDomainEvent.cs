using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "ContactInfoUpdatedDomainEvent")]
public class ContactInfoUpdatedDomainEvent(
     Guid aggregateId,
     ContactInfo contact,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public ContactInfo Contact { get; } = contact;
    public static ContactInfoUpdatedDomainEvent Create(Guid aggregateId, ContactInfo contact)
    {
        return new ContactInfoUpdatedDomainEvent(aggregateId, contact);
    }
}
