namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteUser;

public class DeleteUsersCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.Delete(user.IdUser);

        await repository.DeleteAsync<UserAggregate>(aggregate.Id, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}