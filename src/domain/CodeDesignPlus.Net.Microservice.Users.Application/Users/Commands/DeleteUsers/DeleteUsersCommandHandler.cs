namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.DeleteUsers;

public class DeleteUsersCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<DeleteUsersCommand>
{
    public async Task Handle(DeleteUsersCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.Delete(user.IdUser);

        await repository.DeleteAsync<UsersAggregate>(aggregate.Id, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}