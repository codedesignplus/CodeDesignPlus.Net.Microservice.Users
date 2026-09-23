using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;

public class RemoveRoleCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<RemoveRoleCommand>
{
    public async Task Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        // Se retira con una escritura atomica, no leyendo y guardando el documento entero: si no, una
        // revocacion y una asignacion simultaneas se pisan igual que en AddRoleAsync.
        var result = await repository.RemoveRoleAsync(request.Id, request.TenantId, request.Role, request.IdUser, cancellationToken);

        ApplicationGuard.IsTrue(result == RoleAssignmentResult.TenantNotFound, Errors.TenantNotFound);

        if (result == RoleAssignmentResult.NothingToDo)
            return;

        await pubsub.PublishAsync(
            [RoleRemovedToUserDomainEvent.Create(aggregate.Id, aggregate.DisplayName, request.TenantId, request.Role)],
            cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}
