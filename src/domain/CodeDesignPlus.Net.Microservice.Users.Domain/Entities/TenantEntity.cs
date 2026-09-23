namespace CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

public class TenantEntity : IEntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    /// <summary>
    /// Los grupos del proveedor de identidad que el usuario tiene <b>en esta copropiedad</b>.
    /// </summary>
    /// <remarks>
    /// Un rol se identifica por el id de su grupo y no por su nombre: eso es lo que trae el token en el
    /// claim <c>groups</c> y lo que guarda la audiencia de los avisos.
    /// <para>
    /// Los roles viven aqui y no en la raiz del agregado porque no son del usuario, son del usuario
    /// <b>en una copropiedad</b>: quien administra una y en otra solo reside no puede tener el mismo
    /// papel en las dos. La raiz conserva <c>UserAggregate.Roles</c> para los de plataforma, que son los
    /// unicos que no cuelgan de ninguna.
    /// </para>
    /// </remarks>
    public List<Guid> Roles { get; set; } = [];
}
