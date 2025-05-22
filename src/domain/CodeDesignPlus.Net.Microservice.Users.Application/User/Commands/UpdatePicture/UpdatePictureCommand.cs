namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdatePicture;

[DtoGenerator]
public record UpdatePictureCommand(Guid Id, string Name, string Target) : IRequest;

public class Validator : AbstractValidator<UpdatePictureCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Target).NotEmpty().NotNull();
    }
}
