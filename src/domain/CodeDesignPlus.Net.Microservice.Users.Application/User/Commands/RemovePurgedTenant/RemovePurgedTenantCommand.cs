namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemovePurgedTenant;

/// <summary>
/// Retira una copropiedad purgada a todos sus miembros, con sus roles.
/// </summary>
/// <param name="TenantId">La copropiedad purgada.</param>
public record RemovePurgedTenantCommand(Guid TenantId) : IRequest;

public class Validator : AbstractValidator<RemovePurgedTenantCommand>
{
    public Validator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
