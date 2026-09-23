namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "RoleAddedToUserDomainEvent")]
public class RoleAddedToUserDomainEvent(
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
    /// La copropiedad en la que el usuario pasa a tener ese rol.
    /// </summary>
    /// <remarks>
    /// El proveedor de identidad no sabe que es una copropiedad, asi que quien consuma este evento para
    /// meter al usuario en un grupo tiene que ignorarla a proposito y dejarlo dicho.
    /// </remarks>
    public Guid TenantId { get; } = tenantId;

    /// <summary>
    /// El id del grupo del proveedor de identidad, no el nombre del rol.
    /// </summary>
    public Guid Role { get; } = role;

    public static RoleAddedToUserDomainEvent Create(Guid aggregateId, string? displayName, Guid tenantId, Guid role)
    {
        return new RoleAddedToUserDomainEvent(aggregateId, displayName, tenantId, role);
    }
}
