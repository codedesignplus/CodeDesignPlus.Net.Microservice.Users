using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;

public class AddRoleCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<AddRoleCommand>
{
    public async Task Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        var added = await repository.AddRoleAsync(request.Id, request.Role, request.IdUser, cancellationToken);

        if (!added)
            return;

        await pubsub.PublishAsync(
            [RoleAddedToUserDomainEvent.Create(aggregate.Id, aggregate.DisplayName, request.Role)],
            cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}
