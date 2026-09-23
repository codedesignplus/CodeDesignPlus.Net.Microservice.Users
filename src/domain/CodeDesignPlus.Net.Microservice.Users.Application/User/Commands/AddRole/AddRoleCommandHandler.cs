using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;

public class AddRoleCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<AddRoleCommand>
{
    public async Task Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        var result = await repository.AddRoleAsync(request.Id, request.TenantId, request.Role, request.IdUser, cancellationToken);

        // No se puede dar un papel en una copropiedad a la que el usuario no pertenece. Se comprueba en el
        // filtro de la escritura y no antes, para que no quede ventana entre la comprobacion y el cambio.
        ApplicationGuard.IsTrue(result == RoleAssignmentResult.TenantNotFound, Errors.TenantNotFound);

        if (result == RoleAssignmentResult.NothingToDo)
            return;

        await pubsub.PublishAsync(
            [RoleAddedToUserDomainEvent.Create(aggregate.Id, aggregate.DisplayName, request.TenantId, request.Role)],
            cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}
