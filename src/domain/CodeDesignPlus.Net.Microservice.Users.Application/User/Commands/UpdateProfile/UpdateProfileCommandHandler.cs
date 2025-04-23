namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.UpdateProfile(request.Image, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, request.IsActive, request.Contact, request.Job, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}