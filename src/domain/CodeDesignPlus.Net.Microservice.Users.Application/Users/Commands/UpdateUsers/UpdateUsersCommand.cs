namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateUsers;

[DtoGenerator]
public record UpdateUsersCommand(Guid Id, string FirstName, string LastName, string? DisplayName, string Email, string Phone, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateUsersCommand>
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
