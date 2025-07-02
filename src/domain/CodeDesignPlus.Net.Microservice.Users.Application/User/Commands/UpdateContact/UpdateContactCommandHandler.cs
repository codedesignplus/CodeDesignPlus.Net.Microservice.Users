namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateContact;

public class UpdateContactCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<UpdateContactCommand>
{
    public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.UpdateContactInfo(request.Address, request.City, request.State, request.Country, request.PostalCode, request.Phone, request.Email, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}