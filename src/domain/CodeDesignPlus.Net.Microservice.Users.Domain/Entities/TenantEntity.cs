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
}
