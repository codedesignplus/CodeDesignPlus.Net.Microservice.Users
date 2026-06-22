namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;

[EventKey("UserAggregate", 1, "UserCreatedDomainEvent", "ms-microsoftgraph")]
public class UserCreatedFromSSODomainEvent(
    Guid aggregateId,
    string firstName,
    string lastName,
    string email,
    string phone,
    string? displayName,
    string documentNumber,
    string? passwordKey,
    string? passwordCipher,
    bool wasCreatedFromSSO,
    bool isActive,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
    public string Phone { get; } = phone;
    public string? DisplayName { get; } = displayName;
    public string DocumentNumber { get; } = documentNumber;
    public string? PasswordKey { get; } = passwordKey;
    public string? PasswordCipher { get; } = passwordCipher;
    public bool WasCreatedFromSSO { get; } = wasCreatedFromSSO;
    public bool IsActive { get; } = isActive;
}
