namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;

public class RemoveRoleCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<RemoveRoleCommand>
{
    public async Task Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.RemoveRole(request.Role, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}