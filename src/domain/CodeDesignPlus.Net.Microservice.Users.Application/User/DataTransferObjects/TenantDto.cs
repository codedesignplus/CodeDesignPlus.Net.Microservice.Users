using System;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;

public class TenantDto: IDtoBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    /// <summary>
    /// Los grupos del proveedor de identidad que el usuario tiene en esta copropiedad.
    /// </summary>
    public List<Guid> Roles { get; set; } = [];
}
