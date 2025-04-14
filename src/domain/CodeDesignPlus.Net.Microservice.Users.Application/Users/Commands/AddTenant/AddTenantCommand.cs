namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.AddTenant;

[DtoGenerator]
public record AddTenantCommand(Guid Id, TenantDto Tenant) : IRequest;

public class Validator : AbstractValidator<AddTenantCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Tenant).NotEmpty().NotNull();
    }
}
