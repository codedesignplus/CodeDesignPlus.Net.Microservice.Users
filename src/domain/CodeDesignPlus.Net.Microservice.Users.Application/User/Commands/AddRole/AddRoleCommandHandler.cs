namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;

public class AddRoleCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<AddRoleCommand>
{
    public async Task Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.AddRole(request.Role, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}