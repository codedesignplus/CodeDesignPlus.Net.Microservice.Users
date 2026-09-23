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

        // Que el usuario no pertenezca a esa copropiedad no es un error al retirar: significa que no tiene
        // nada que perder alli. Tratarlo como fallo llenaria la cola de descartes con revocaciones que ya
        // estaban cumplidas.
        if (result is RoleAssignmentResult.NothingToDo or RoleAssignmentResult.TenantNotFound)
            return;

        // Se relee despues de escribir, no antes: lo que importa es como quedo el usuario, y el agregado
        // que se leyo al principio todavia tiene el rol puesto.
        var despues = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        var leQuedaEnOtra = despues is not null
            && despues.Tenants.Exists(x => x.Id != request.TenantId && x.Roles.Contains(request.Role));

        await pubsub.PublishAsync(
            [RoleRemovedToUserDomainEvent.Create(aggregate.Id, aggregate.DisplayName, request.TenantId, request.Role, leQuedaEnOtra)],
            cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }
}
