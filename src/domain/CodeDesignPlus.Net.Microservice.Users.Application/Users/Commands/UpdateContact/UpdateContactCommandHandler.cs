namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateContact;

public class UpdateContactCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<UpdateContactCommand>
{
    public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.UpdateContactInfo(request.Address, request.City, request.State, request.Country, request.PostalCode, request.Phone, request.Email, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}