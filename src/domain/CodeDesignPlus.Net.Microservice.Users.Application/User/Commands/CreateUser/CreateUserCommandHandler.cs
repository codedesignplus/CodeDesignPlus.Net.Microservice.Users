namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository, IPubSub pubsub) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        // Ya guardado por un envío anterior del mismo formulario: el reintento termina bien sin repetir nada (pendings/589).
        if (await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken))
            return;

        // Un correo, una cuenta (pendings/215): sin esto, invitar dos veces a la misma persona creaba un segundo
        // usuario que Entra rechazaba después, en segundo plano.
        var emailTaken = await repository.ExistsByEmailAsync(request.Email, cancellationToken);

        ApplicationGuard.IsTrue(emailTaken, Errors.UserEmailAlreadyExists);

        var aggregate = UserAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, request.DocumentNumber, request.DocumentType, request.IsActive);

        // Dos envíos que llegan a la vez pasan los dos la comprobación de arriba; el segundo choca contra el id y no publica.
        if (!await repository.CreateIfAbsentAsync(aggregate, cancellationToken))
            return;

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}