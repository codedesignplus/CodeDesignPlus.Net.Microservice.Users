namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.CreateUsers;

[DtoGenerator]
public record CreateUsersCommand(Guid Id, string FirstName, string LastName, string? DisplayName, string Email, string Phone) : IRequest;

public class Validator : AbstractValidator<CreateUsersCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
    }
}
