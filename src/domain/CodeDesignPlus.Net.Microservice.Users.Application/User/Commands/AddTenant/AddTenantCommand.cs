namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;

[DtoGenerator]
public record AddTenantCommand(Guid UserId, TenantDto Tenant) : IRequest;

public class Validator : AbstractValidator<AddTenantCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty().NotNull();
        RuleFor(x => x.Tenant).NotEmpty().NotNull();
    }
}
