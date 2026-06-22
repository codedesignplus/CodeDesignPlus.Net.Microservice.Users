namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

/// <summary>
/// Raised when a new <see cref="UserAggregate"/> is created in ms-users.
///
/// Named "Registered" (not "Created") on purpose: ms-microsoftgraph already owns its own
/// <c>UserCreatedDomainEvent</c> for the Azure AD identity. Using a distinct verb keeps
/// both events serializable side-by-side in the same RabbitMQ broker without colliding
/// at the consumer's <c>DomainEventResolverService</c>, which keys events solely by the
/// event name (the SDK ignores the cross-service AppName when registering types in its
/// internal dictionary).
/// </summary>
[EventKey<UserAggregate>(1, "UserRegisteredDomainEvent")]
public class UserRegisteredDomainEvent : UserBaseDomainEvent
{
    public UserRegisteredDomainEvent(
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

    public static UserRegisteredDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, Item<string>? documentType, bool isActive)
    {
        return new UserRegisteredDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, documentNumber, documentType, isActive);
    }
}
