namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;

/// <summary>
/// Copia local del alta que publica ms-microsoftgraph cuando alguien se registra por el flujo de Entra External ID.
/// </summary>
/// <remarks>
/// Trae el tipo de documento desde el plan 663: ms-microsoftgraph lo lee del registro y lo resuelve contra el catalogo,
/// y ms-users lo exige al crear el usuario.
/// </remarks>
[EventKey("UserAggregate", 1, "UserCreatedDomainEvent", "ms-microsoftgraph")]
public class UserCreatedFromSSODomainEvent(
    Guid aggregateId,
    string firstName,
    string lastName,
    string email,
    string phone,
    string? displayName,
    string documentNumber,
    DocumentType documentType,
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
    public DocumentType DocumentType { get; } = documentType;
    public string? PasswordKey { get; } = passwordKey;
    public string? PasswordCipher { get; } = passwordCipher;
    public bool WasCreatedFromSSO { get; } = wasCreatedFromSSO;
    public bool IsActive { get; } = isActive;
}
