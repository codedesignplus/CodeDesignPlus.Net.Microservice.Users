namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;

[DtoGenerator]
public record CreateUserCommand(Guid Id, string FirstName, string LastName, string? DisplayName, string Email, string Phone, string PasswordKey, string PasswordCipher, bool IsActive) : IRequest;

public class Validator : AbstractValidator<CreateUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
        RuleFor(x => x.PasswordKey).NotEmpty().NotNull();
        RuleFor(x => x.PasswordCipher).NotEmpty().NotNull();
    }
}
