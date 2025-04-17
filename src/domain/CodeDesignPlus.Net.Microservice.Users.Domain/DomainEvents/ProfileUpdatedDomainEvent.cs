using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UsersAggregate>(1, "ProfileUpdatedDomainEvent")]
public class ProfileUpdatedDomainEvent(
     Guid aggregateId,
     ContactInfo contact,
     JobInfo job,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) :  UserBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public ContactInfo Contact { get; } = contact;
    public JobInfo Job { get; } = job;

    public static ProfileUpdatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, bool isActive, ContactInfo contact, JobInfo job)
    {
        return new ProfileUpdatedDomainEvent(aggregateId, contact, job)
        {
            FirtName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DisplayName = displayName,
            IsActive = isActive,
        };
    }
}
