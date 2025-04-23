namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;

public class AddTenantCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<AddTenantCommand>
{
    public async Task Handle(AddTenantCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.AddTenant(request.Tenant.Id, request.Tenant.Name, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}