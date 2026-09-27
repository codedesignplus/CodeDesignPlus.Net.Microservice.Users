namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.SetTenantPurgeAfter;

/// <summary>
/// Marca en todos sus miembros que una copropiedad se eliminó, o quita la marca si se restauró.
/// </summary>
/// <remarks>
/// No toca roles ni publica eventos: mientras dure el plazo el usuario sigue en los grupos de Entra, y restaurar la
/// copropiedad se la devuelve tal como estaba. Se invalida la caché de cada miembro, porque <c>/init</c> lee el usuario
/// de ahí.
/// </remarks>
public class SetTenantPurgeAfterCommandHandler(IUserRepository repository, ICacheManager cacheManager) : IRequestHandler<SetTenantPurgeAfterCommand>
{
    public async Task Handle(SetTenantPurgeAfterCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var members = await repository.FindByTenantAsync(request.TenantId, cancellationToken);

        foreach (var member in members)
        {
            await repository.SetTenantPurgeAfterAsync(member.Id, request.TenantId, request.PurgeAfter, cancellationToken);

            var exist = await cacheManager.ExistsAsync(member.Id.ToString());

            if (exist)
            {
                await cacheManager.RemoveAsync(member.Id.ToString());
            }
        }
    }
}
