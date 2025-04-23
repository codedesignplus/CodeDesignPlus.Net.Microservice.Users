namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteUser;


public record DeleteUserCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<DeleteUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
