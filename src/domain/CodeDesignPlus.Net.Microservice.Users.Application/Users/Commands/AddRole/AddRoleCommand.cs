namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.AddRole;

[DtoGenerator]
public record AddRoleCommand(Guid Id, string Role) : IRequest;

public class Validator : AbstractValidator<AddRoleCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
    }
}
