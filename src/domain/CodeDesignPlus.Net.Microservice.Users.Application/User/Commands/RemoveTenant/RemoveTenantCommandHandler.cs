namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveTenant;

public class RemoveTenantCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<RemoveTenantCommand>
{
    public async Task Handle(RemoveTenantCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.RemoveTenant(request.IdTenant, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}