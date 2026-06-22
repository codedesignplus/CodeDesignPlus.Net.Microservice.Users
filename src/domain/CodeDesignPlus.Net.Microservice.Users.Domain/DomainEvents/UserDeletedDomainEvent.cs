namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "UserDeletedDomainEvent")]
public class UserDeletedDomainEvent : UserBaseDomainEvent
{
    public UserDeletedDomainEvent(
        Guid aggregateId,
        string firstName,
        string lastName,
        string email,
        string phone,
        string? displayName,
        string documentNumber,
        Item<string>? documentType,
        bool isActive,
        Guid? eventId = null,
        Instant? occurredAt = null,
        Dictionary<string, object>? metadata = null
    ) : base(aggregateId, eventId, occurredAt, metadata)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        DocumentNumber = documentNumber;
        DocumentType = documentType;
        IsActive = isActive;
    }

    public static UserDeletedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, Item<string>? documentType, bool isActive)
    {
        return new UserDeletedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, documentNumber, documentType, isActive);
    }
}
