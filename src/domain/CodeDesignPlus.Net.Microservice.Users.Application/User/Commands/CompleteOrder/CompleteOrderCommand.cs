namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CompleteOrder;

[DtoGenerator]
public record CompleteOrderCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<CompleteOrderCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
