namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.CreateUsers;

public class CreateUsersCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<CreateUsersCommand>
{
    public async Task Handle(CreateUsersCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);
        
        var exist = await repository.ExistsAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var aggregate = UsersAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone,  request.DisplayName, []);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}