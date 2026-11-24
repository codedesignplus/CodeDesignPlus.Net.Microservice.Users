namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteReleasedUser;

/// <summary>
/// Borra la cuenta que el sistema desactivó porque al usuario no le quedaba nada, si sigue así.
/// </summary>
/// <remarks>
/// Es la segunda mitad de <c>RemoveRoleCommand.KeepAccount</c>: quien retiró el último rol sin borrar la cuenta la borra
/// cuando ya no la necesita. Si entretanto el usuario recibió un rol (la cuenta se reactivó) o un administrador la
/// desactivó a mano, no se toca: la cuenta ya no es de quien la pidió borrar. El borrado publica
/// <c>UserDeletedDomainEvent</c>, con el que ms-microsoftgraph borra la cuenta en Entra, igual que el borrado de siempre.
/// </remarks>
/// <param name="repository">El repositorio de usuarios.</param>
/// <param name="pubsub">El bus de eventos.</param>
/// <param name="cacheManager">La caché de usuarios.</param>
public class DeleteReleasedUserCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<DeleteReleasedUserCommand, bool>
{
    /// <summary>
    /// Borra la cuenta si sigue desactivada por no tener nada.
    /// </summary>
    /// <param name="request">El comando.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>true</c> si se borró; <c>false</c> si no existe o ya no está en ese estado.</returns>
    public async Task<bool> Handle(DeleteReleasedUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var user = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        if (user is null || !user.IsReleasedAndUnused())
        {
            return false;
        }

        user.Delete(request.IdUser);

        await repository.DeleteAsync<UserAggregate>(user.Id, cancellationToken);

        await pubsub.PublishAsync(user.GetAndClearEvents(), cancellationToken);

        if (await cacheManager.ExistsAsync(request.Id.ToString()))
        {
            await cacheManager.RemoveAsync(request.Id.ToString());
        }

        return true;
    }
}
