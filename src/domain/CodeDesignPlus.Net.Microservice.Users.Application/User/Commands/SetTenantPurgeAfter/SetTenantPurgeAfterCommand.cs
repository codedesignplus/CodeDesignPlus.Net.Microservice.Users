namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.SetTenantPurgeAfter;

/// <summary>
/// Marca en todos sus miembros que una copropiedad se eliminó, con su fecha de purga, o quita la marca si se restauró.
/// </summary>
/// <param name="TenantId">La copropiedad.</param>
/// <param name="PurgeAfter">Cuándo se purga, o <c>null</c> si se restauró.</param>
public record SetTenantPurgeAfterCommand(Guid TenantId, Instant? PurgeAfter) : IRequest;

public class Validator : AbstractValidator<SetTenantPurgeAfterCommand>
{
    public Validator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
