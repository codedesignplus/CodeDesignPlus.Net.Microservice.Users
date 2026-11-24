namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteReleasedUser;

/// <summary>
/// Borra la cuenta que el sistema desactivó porque al usuario no le quedaba nada, si sigue así.
/// </summary>
/// <param name="Id">El usuario.</param>
/// <param name="IdUser">Quien lo pide.</param>
public record DeleteReleasedUserCommand(Guid Id, Guid IdUser) : IRequest<bool>;

/// <summary>
/// Valida el comando.
/// </summary>
public class Validator : AbstractValidator<DeleteReleasedUserCommand>
{
    /// <summary>
    /// Exige el usuario y quien lo pide.
    /// </summary>
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.IdUser).NotEmpty().NotNull();
    }
}
