namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);
        
        var exist = await repository.ExistsAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var aggregate = UsersAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone,  request.DisplayName, []);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}