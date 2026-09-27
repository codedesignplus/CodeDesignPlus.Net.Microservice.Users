namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemovePurgedTenant;

/// <summary>
/// Retira una copropiedad purgada a todos sus miembros, con sus roles.
/// </summary>
/// <remarks>
/// Los eventos salen <b>antes</b> de quitar la copropiedad. Al reves, un fallo entre los dos pasos dejaria al
/// usuario fuera de la copropiedad pero dentro de los grupos de Entra, y el reintento ya no lo encontraria como
/// miembro. Asi el reintento lo vuelve a encontrar y vuelve a publicar; retirar de un grupo a quien ya no esta
/// es inofensivo.
/// </remarks>
public class RemovePurgedTenantCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<RemovePurgedTenantCommand>
{
    public async Task Handle(RemovePurgedTenantCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var members = await repository.FindByTenantAsync(request.TenantId, cancellationToken);

        foreach (var member in members)
        {
            member.RemovePurgedTenant(request.TenantId);

            await pubsub.PublishAsync(member.GetAndClearEvents(), cancellationToken);

            await repository.RemoveTenantAsync(member.Id, request.TenantId, cancellationToken);

            var exist = await cacheManager.ExistsAsync(member.Id.ToString());

            if (exist)
            {
                await cacheManager.RemoveAsync(member.Id.ToString());
            }
        }
    }
}
