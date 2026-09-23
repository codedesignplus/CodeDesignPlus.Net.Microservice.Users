namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "RoleRemovedToUserDomainEvent")]
public class RoleRemovedToUserDomainEvent(
     Guid aggregateId,
     string? displayName,
     Guid tenantId,
     Guid role,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// La copropiedad en la que el usuario deja de tener ese rol.
    /// </summary>
    /// <remarks>
    /// Retirar el rol de una copropiedad no lo retira de las demas: antes de sacar al usuario del grupo
    /// del proveedor de identidad hay que comprobar que no le queda en ninguna otra.
    /// </remarks>
    public Guid TenantId { get; } = tenantId;

    /// <summary>
    /// El id del grupo del proveedor de identidad, no el nombre del rol.
    /// </summary>
    public Guid Role { get; } = role;

    public static RoleRemovedToUserDomainEvent Create(Guid aggregateId, string? displayName, Guid tenantId, Guid role)
    {
        return new RoleRemovedToUserDomainEvent(aggregateId, displayName, tenantId, role);
    }
}
