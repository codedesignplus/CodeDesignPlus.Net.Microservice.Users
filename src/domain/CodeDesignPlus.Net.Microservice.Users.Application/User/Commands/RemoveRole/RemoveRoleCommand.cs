namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;


public record RemoveRoleCommand(Guid Id, string Role) : IRequest;

public class Validator : AbstractValidator<RemoveRoleCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
    }
}
