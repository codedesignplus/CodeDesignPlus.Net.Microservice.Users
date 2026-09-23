namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;

/// <summary>
/// Asigna un rol a un usuario en una copropiedad.
/// </summary>
/// <param name="Id">El usuario.</param>
/// <param name="TenantId">La copropiedad en la que tendra el rol. Un rol sin copropiedad no existe.</param>
/// <param name="Role">El id del grupo del proveedor de identidad, no el nombre del rol.</param>
/// <param name="IdUser">Quien lo asigna.</param>
[DtoGenerator]
public record AddRoleCommand(Guid Id, Guid TenantId, Guid Role, Guid IdUser) : IRequest;

public class Validator : AbstractValidator<AddRoleCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.TenantId).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
        RuleFor(x => x.IdUser).NotEmpty().NotNull();
    }
}
