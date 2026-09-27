namespace CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

public class TenantEntity : IEntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    /// <summary>
    /// Los roles que el usuario tiene <b>en esta copropiedad</b>, por su id en el catalogo.
    /// </summary>
    /// <remarks>
    /// Un rol se identifica por su id en el catalogo de ms-roles, que es el mismo en todos los entornos.
    /// El id del grupo en el proveedor de identidad cambia con cada directorio y solo lo necesita
    /// ms-microsoftgraph, que lo resuelve a partir de este.
    /// <para>
    /// Los roles viven aqui y no en la raiz del agregado porque no son del usuario, son del usuario
    /// <b>en una copropiedad</b>: quien administra una y en otra solo reside no puede tener el mismo
    /// papel en las dos. La raiz conserva <c>UserAggregate.Roles</c> para los de plataforma, que son los
    /// unicos que no cuelgan de ninguna.
    /// </para>
    /// </remarks>
    public List<Guid> Roles { get; set; } = [];

    /// <summary>
    /// Si la copropiedad está eliminada, cuándo se purgan sus datos; <c>null</c> mientras exista.
    /// </summary>
    /// <remarks>
    /// Es una réplica de ms-tenants, que la mantiene con <c>TenantDeleted</c> y <c>TenantRestored</c>. La membresía no
    /// se quita al eliminar la copropiedad, porque entonces restaurarla no se la devolvería a sus miembros: se marca, y
    /// el frontend no ofrece en <c>/init</c> una copropiedad marcada. Al purgarse sí se retira (<c>TenantPurged</c>).
    /// </remarks>
    public Instant? PurgeAfter { get; set; }
}
