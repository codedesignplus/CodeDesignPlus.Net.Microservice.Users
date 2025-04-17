namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.Update(request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, request.IsActive, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}