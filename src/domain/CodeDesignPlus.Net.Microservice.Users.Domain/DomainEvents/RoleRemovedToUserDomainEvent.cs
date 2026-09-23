namespace CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

[EventKey<UserAggregate>(1, "RoleRemovedToUserDomainEvent")]
public class RoleRemovedToUserDomainEvent(
     Guid aggregateId,
     string? displayName,
     Guid tenantId,
     Guid role,
     bool stillHasItElsewhere,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;

    /// <summary>
    /// La copropiedad en la que el usuario deja de tener ese rol.
    /// </summary>
    public Guid TenantId { get; } = tenantId;

    /// <summary>
    /// El id del grupo del proveedor de identidad, no el nombre del rol.
    /// </summary>
    public Guid Role { get; } = role;

    /// <summary>
    /// Si al usuario le queda ese mismo rol en alguna otra copropiedad.
    /// </summary>
    /// <remarks>
    /// <b>Es lo que decide si se le saca del grupo del proveedor de identidad</b>, que es global y no
    /// sabe de copropiedades. Sin este dato, quitarle "Residente" en una copropiedad lo sacaria del grupo
    /// y perderia el papel en todas las demas.
    /// <para>
    /// Lo calcula quien publica porque es el unico que tiene el documento entero: preguntarlo desde el
    /// consumidor seria una llamada cruzada para responder algo que aqui ya se sabe.
    /// </para>
    /// </remarks>
    public bool StillHasItElsewhere { get; } = stillHasItElsewhere;

    public static RoleRemovedToUserDomainEvent Create(Guid aggregateId, string? displayName, Guid tenantId, Guid role, bool stillHasItElsewhere)
    {
        return new RoleRemovedToUserDomainEvent(aggregateId, displayName, tenantId, role, stillHasItElsewhere);
    }
}
