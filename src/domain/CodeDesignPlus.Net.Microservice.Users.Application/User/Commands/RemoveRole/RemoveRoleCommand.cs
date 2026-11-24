namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;

/// <summary>
/// Retira un rol de un usuario en una copropiedad.
/// </summary>
/// <param name="Id">El usuario.</param>
/// <param name="TenantId">La copropiedad de la que se retira. Retirarlo de una no lo retira de las demas.</param>
/// <param name="Role">El id del rol en el catalogo, que es el mismo en todos los entornos.</param>
/// <param name="IdUser">Quien lo retira.</param>
/// <param name="KeepAccount">
/// Si al usuario no le queda nada, desactiva la cuenta en vez de borrarla. Por omisión se borra, como siempre.
/// </param>
public record RemoveRoleCommand(Guid Id, Guid TenantId, Guid Role, Guid IdUser, bool KeepAccount = false) : IRequest;

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
