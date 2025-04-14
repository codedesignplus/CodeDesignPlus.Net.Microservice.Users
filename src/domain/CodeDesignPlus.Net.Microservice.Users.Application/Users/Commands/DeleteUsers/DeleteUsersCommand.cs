namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.DeleteUsers;

[DtoGenerator]
public record DeleteUsersCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<DeleteUsersCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
