namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);
        
        var exist = await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var aggregate = UserAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, user.IdUser);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}