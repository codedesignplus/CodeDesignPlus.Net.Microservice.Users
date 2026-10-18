using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

/// <summary>
/// La foto de un usuario: lo mismo que publica <see cref="ProfileUpdatedDomainEvent"/>, sin el empleo, para la copia local
/// de usuarios que la pida al arrancar.
/// </summary>
/// <remarks>
/// La publica el SDK solo cuando una copia la pide, dirigida a ella; sirve a cualquier micro que proyecte usuarios y se
/// haya creado despues que ellos. El usuario es de la plataforma y no de una copropiedad: la foto va con
/// <see cref="Guid.Empty"/> como tenant.
/// </remarks>
/// <param name="aggregateId">El usuario.</param>
/// <param name="tenant">Siempre <see cref="Guid.Empty"/>: el usuario es de la plataforma.</param>
/// <param name="firstName">Los nombres.</param>
/// <param name="lastName">Los apellidos.</param>
/// <param name="email">El correo.</param>
/// <param name="phone">El telefono.</param>
/// <param name="displayName">El nombre para mostrar.</param>
/// <param name="documentNumber">El numero del documento.</param>
/// <param name="documentType">El tipo de documento, si lo tiene.</param>
/// <param name="isActive">Si esta activo.</param>
/// <param name="contact">Los datos de contacto, si los tiene.</param>
/// <param name="eventId">El identificador del evento (opcional).</param>
/// <param name="occurredAt">Cuando se leyo el usuario (opcional).</param>
/// <param name="metadata">Metadatos adicionales (opcional).</param>
[EventKey<UserAggregate>(1, "UserSnapshotDomainEvent")]
public class UserSnapshotDomainEvent(
    Guid aggregateId,
    Guid tenant,
    string firstName,
    string lastName,
    string email,
    string phone,
    string? displayName,
    string documentNumber,
    DocumentType? documentType,
    bool isActive,
    ContactInfo? contact,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : SnapshotDomainEvent(aggregateId, tenant, eventId, occurredAt, metadata)
{
    /// <summary>Los nombres.</summary>
    public string FirstName { get; } = firstName;

    /// <summary>Los apellidos.</summary>
    public string LastName { get; } = lastName;

    /// <summary>El correo.</summary>
    public string Email { get; } = email;

    /// <summary>El telefono.</summary>
    public string Phone { get; } = phone;

    /// <summary>El nombre para mostrar.</summary>
    public string? DisplayName { get; } = displayName;

    /// <summary>El numero del documento.</summary>
    public string DocumentNumber { get; } = documentNumber;

    /// <summary>El tipo de documento, si lo tiene.</summary>
    public DocumentType? DocumentType { get; } = documentType;

    /// <summary>Si esta activo.</summary>
    public bool IsActive { get; } = isActive;

    /// <summary>Los datos de contacto, si los tiene.</summary>
    public ContactInfo? Contact { get; } = contact;
}
