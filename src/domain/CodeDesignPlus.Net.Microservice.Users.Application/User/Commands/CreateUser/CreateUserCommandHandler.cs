namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository, IPubSub pubsub) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);
        
        var exist = await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        // Un correo, una cuenta (pendings/215): sin esto, invitar dos veces a la misma persona creaba un segundo
        // usuario que Entra rechazaba después, en segundo plano.
        var emailTaken = await repository.ExistsByEmailAsync(request.Email, cancellationToken);

        ApplicationGuard.IsTrue(emailTaken, Errors.UserEmailAlreadyExists);

        var aggregate = UserAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, request.DocumentNumber, request.DocumentType, request.IsActive);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}