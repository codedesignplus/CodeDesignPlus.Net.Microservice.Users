namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateContact;

[DtoGenerator]
public record UpdateContactCommand(Guid Id, string Address, string City, string State, string Country, string PostalCode, string Phone, string[] Email) : IRequest;

public class Validator : AbstractValidator<UpdateContactCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
