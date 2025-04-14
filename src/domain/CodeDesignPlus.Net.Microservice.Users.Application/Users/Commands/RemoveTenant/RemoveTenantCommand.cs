namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.RemoveTenant;

[DtoGenerator]
public record RemoveTenantCommand(Guid Id, Guid IdTenant) : IRequest;

public class Validator : AbstractValidator<RemoveTenantCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.IdTenant).NotEmpty().NotNull();
    }
}
