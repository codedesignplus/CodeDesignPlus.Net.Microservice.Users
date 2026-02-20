using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "ProfileUpdatedDomainEvent", autoCreate: false)]
public class ProfileUpdatedDomainEvent : UserBaseDomainEvent
{
    public ContactInfo Contact { get; }
    public JobInfo Job { get; }

    public ProfileUpdatedDomainEvent(
        Guid aggregateId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string? displayName,
        bool isActive,
        ContactInfo contact,
        JobInfo job,
        Guid? eventId = null,
        Instant? occurredAt = null,
        Dictionary<string, object>? metadata = null
    ) : base(aggregateId, eventId, occurredAt, metadata)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.Phone = phone;
        this.DisplayName = displayName;
        this.IsActive = isActive;
        this.Contact = contact;
        this.Job = job;
    }

    public static ProfileUpdatedDomainEvent Create(Guid aggregateId,  string firstName, string lastName, string email, string phone, string? displayName, bool isActive, ContactInfo contact, JobInfo job)
    {
        return new ProfileUpdatedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, isActive, contact, job);
    }
}
