using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;

/// <summary>
/// Asigna un rol a un usuario en una copropiedad.
/// </summary>
/// <remarks>
/// <b>Darle un papel en una copropiedad es como se entra en ella.</b> Un propietario no pertenece primero
/// a la copropiedad y luego recibe su titulo: el titulo es la entrada. Por eso, si el usuario todavia no
/// esta en ella, se le anade en vez de rechazar la operacion.
/// <para>
/// Lo que si se rechaza es una copropiedad que no se puede resolver. Como el directorio no distingue
/// «no existe» de «ahora mismo no puedo comprobarlo», se falla en los dos casos y se deja que el
/// reintento decida: si el dato era malo la cola acabara en la de descartes, donde se ve; si era una
/// caida pasajera, el siguiente intento lo resuelve. Tratarlo como inexistente perderia la asignacion en
/// silencio por un corte de Redis.
/// </para>
/// </remarks>
public class AddRoleCommandHandler(
    IUserRepository repository,
    ITenantDirectory tenantDirectory,
    IPubSub pubsub,
    ICacheManager cacheManager) : IRequestHandler<AddRoleCommand>
{
    public async Task Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        var result = await repository.AddRoleAsync(request.Id, request.TenantId, request.Role, request.IdUser, cancellationToken);

        // Solo se consulta el directorio cuando hace falta: el camino normal -el usuario ya pertenece- no
        // paga ninguna consulta.
        if (result == RoleAssignmentResult.TenantNotFound)
            result = await JoinTenantAndRetryAsync(request, cancellationToken);

        if (result == RoleAssignmentResult.NothingToDo)
            return;

        await pubsub.PublishAsync(
            [RoleAddedToUserDomainEvent.Create(aggregate.Id, aggregate.DisplayName, request.TenantId, request.Role)],
            cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.Id.ToString());
    }

    /// <summary>
    /// Mete al usuario en la copropiedad y vuelve a intentar el rol.
    /// </summary>
    /// <remarks>
    /// El nombre lo pone el directorio y no quien llama: los consumidores que asignan un rol solo tienen
    /// el identificador —les llega en el evento— y si cada uno lo buscara por su cuenta acabariamos con
    /// nombres distintos para la misma copropiedad.
    /// </remarks>
    private async Task<RoleAssignmentResult> JoinTenantAndRetryAsync(AddRoleCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await tenantDirectory.GetSnapshotAsync(request.TenantId, cancellationToken);

        ApplicationGuard.IsNull(snapshot, Errors.TenantCouldNotBeResolved);

        await repository.AddTenantAsync(
            request.Id,
            new TenantEntity { Id = request.TenantId, Name = snapshot.Name },
            request.IdUser,
            cancellationToken);

        var result = await repository.AddRoleAsync(request.Id, request.TenantId, request.Role, request.IdUser, cancellationToken);

        // Si sigue sin casar, el documento cambio entre las dos escrituras y el reintento de la cola lo
        // resolvera. Callarlo dejaria al usuario sin su papel sin que nada lo dijera.
        ApplicationGuard.IsTrue(result == RoleAssignmentResult.TenantNotFound, Errors.TenantCouldNotBeResolved);

        return result;
    }
}
