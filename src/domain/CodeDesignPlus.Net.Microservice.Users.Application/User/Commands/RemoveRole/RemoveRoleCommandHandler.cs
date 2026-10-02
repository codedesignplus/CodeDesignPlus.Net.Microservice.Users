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

        if (despues is not null)
            await ReleaseIfNothingLeftAsync(despues, request, cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }

    /// <summary>
    /// Lo que queda del usuario tras retirarle un rol (decisión del usuario, pendings/211). Si en esa copropiedad ya no
    /// le queda ningún rol, deja de pertenecer a ella; y si además no le queda ninguna copropiedad ni rol de
    /// plataforma, se borra. El borrado publica <c>UserDeletedDomainEvent</c>, con el que ms-microsoftgraph borra
    /// su cuenta en Entra.
    /// </summary>
    /// <remarks>
    /// Vale para toda revocación, no solo la de portería: un contador, un consejero o un propietario que se queda
    /// sin papel en ninguna copropiedad no tiene para qué conservar la cuenta.
    /// </remarks>
    private async Task ReleaseIfNothingLeftAsync(UserAggregate user, RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var tenant = user.Tenants.Find(x => x.Id == request.TenantId);

        if (tenant is null || tenant.Roles.Count > 0)
            return;

        user.RemoveTenant(request.TenantId, request.IdUser);

        if (user.Tenants.Count == 0 && user.Roles.Length == 0)
        {
            user.Delete(request.IdUser);

            await repository.DeleteAsync<UserAggregate>(user.Id, cancellationToken);
        }
        else
        {
            await repository.UpdateAsync(user, cancellationToken);
        }

        await pubsub.PublishAsync(user.GetAndClearEvents(), cancellationToken);
    }
}
