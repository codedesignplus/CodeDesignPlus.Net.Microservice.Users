using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Snapshots;

/// <summary>
/// Contesta la foto de los usuarios que pide al arrancar una copia local de usuarios: cada usuario vivo con su nombre, su
/// documento y su contacto.
/// </summary>
/// <remarks>
/// Con copropiedad, solo sus miembros: sirve para reparar la copia de una sola sin esperar al siguiente arranque.
/// </remarks>
/// <param name="repository">Los usuarios.</param>
public class UserSnapshotSource(IUserRepository repository) : ISnapshotSource<UserSnapshotDomainEvent>
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<UserSnapshotDomainEvent>> GetPageAsync(Guid? tenant, int page, int pageSize, CancellationToken cancellationToken)
    {
        var users = await repository.GetSnapshotPageAsync(tenant, page, pageSize, cancellationToken);

        return [.. users.Select(user => user.TakeSnapshot())];
    }
}
