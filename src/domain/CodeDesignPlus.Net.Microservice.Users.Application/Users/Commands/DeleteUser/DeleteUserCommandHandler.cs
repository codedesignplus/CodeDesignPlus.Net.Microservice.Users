namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.Delete(user.IdUser);

        await repository.DeleteAsync<UsersAggregate>(aggregate.Id, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}