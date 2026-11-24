namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

/// <summary>
/// La cuenta del usuario se desactivó porque no le quedó nada: ninguna copropiedad ni rol de plataforma.
/// </summary>
/// <remarks>
/// Con él, ms-microsoftgraph deshabilita la cuenta en el proveedor de identidad: el usuario deja de poder iniciar sesión,
/// pero la cuenta sigue existiendo.
/// </remarks>
/// <param name="aggregateId">El usuario.</param>
/// <param name="displayName">Su nombre.</param>
/// <param name="eventId">El identificador del evento (opcional).</param>
/// <param name="occurredAt">Cuándo ocurrió (opcional).</param>
/// <param name="metadata">Metadatos adicionales (opcional).</param>
[EventKey<UserAggregate>(1, "UserDeactivatedDomainEvent")]
public class UserDeactivatedDomainEvent(
    Guid aggregateId,
    string? displayName,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    /// <summary>
    /// El nombre del usuario.
    /// </summary>
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// Crea el evento.
    /// </summary>
    /// <param name="aggregateId">El usuario.</param>
    /// <param name="displayName">Su nombre.</param>
    /// <returns>El evento.</returns>
    public static UserDeactivatedDomainEvent Create(Guid aggregateId, string? displayName)
    {
        return new UserDeactivatedDomainEvent(aggregateId, displayName);
    }
}
