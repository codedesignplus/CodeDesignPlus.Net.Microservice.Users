namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

/// <summary>
/// La cuenta que se había desactivado por no tener nada vuelve a estar activa, porque el usuario recibió un rol.
/// </summary>
/// <remarks>
/// Con él, ms-microsoftgraph vuelve a habilitar la cuenta en el proveedor de identidad.
/// </remarks>
/// <param name="aggregateId">El usuario.</param>
/// <param name="displayName">Su nombre.</param>
/// <param name="eventId">El identificador del evento (opcional).</param>
/// <param name="occurredAt">Cuándo ocurrió (opcional).</param>
/// <param name="metadata">Metadatos adicionales (opcional).</param>
[EventKey<UserAggregate>(1, "UserReactivatedDomainEvent")]
public class UserReactivatedDomainEvent(
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
    public static UserReactivatedDomainEvent Create(Guid aggregateId, string? displayName)
    {
        return new UserReactivatedDomainEvent(aggregateId, displayName);
    }
}
