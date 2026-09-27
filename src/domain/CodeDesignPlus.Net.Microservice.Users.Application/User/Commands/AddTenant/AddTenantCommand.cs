namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;

[DtoGenerator]
public record AddTenantCommand(Guid UserId, TenantDto Tenant) : IRequest
{
    /// <summary>
    /// Adds a tenant the user has just bought (see <see cref="ByPurchase"/>).
    /// </summary>
    public AddTenantCommand(Guid userId, TenantDto tenant, bool byPurchase) : this(userId, tenant)
    {
        ByPurchase = byPurchase;
    }

    /// <summary>
    /// The user bought the tenant: ms-emails then sends "your tenant is ready" instead of an invitation.
    /// Read-only so the DtoGenerator leaves it out of the REST DTO: only the purchase flow sets it.
    /// </summary>
    public bool ByPurchase { get; }
}

public class Validator : AbstractValidator<AddTenantCommand>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty().NotNull();
        RuleFor(x => x.Tenant).NotEmpty().NotNull();
    }
}
