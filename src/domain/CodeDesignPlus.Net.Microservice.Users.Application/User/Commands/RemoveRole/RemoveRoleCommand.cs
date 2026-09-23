namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;

/// <summary>
/// Retira un rol de un usuario en una copropiedad.
/// </summary>
/// <param name="Id">El usuario.</param>
/// <param name="TenantId">La copropiedad de la que se retira. Retirarlo de una no lo retira de las demas.</param>
/// <param name="Role">El id del grupo del proveedor de identidad, no el nombre del rol.</param>
/// <param name="IdUser">Quien lo retira.</param>
public record RemoveRoleCommand(Guid Id, Guid TenantId, Guid Role, Guid IdUser) : IRequest;

public class Validator : AbstractValidator<RemoveRoleCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.TenantId).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
        RuleFor(x => x.IdUser).NotEmpty().NotNull();
    }
}
