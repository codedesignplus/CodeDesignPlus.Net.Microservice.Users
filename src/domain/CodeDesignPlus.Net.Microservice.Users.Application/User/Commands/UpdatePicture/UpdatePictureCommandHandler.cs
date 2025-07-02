namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdatePicture;

public class UpdatePictureCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<UpdatePictureCommand>
{
    public async Task Handle(UpdatePictureCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.UpdatePicture(request.Id, request.Name, request.Target, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}