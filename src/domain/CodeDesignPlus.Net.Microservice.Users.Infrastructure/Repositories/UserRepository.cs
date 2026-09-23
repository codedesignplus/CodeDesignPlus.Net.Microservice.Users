using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    /// <inheritdoc/>
    public Task<RoleAssignmentResult> AddRoleAsync(Guid id, Guid tenantId, Guid role, Guid updatedBy, CancellationToken cancellationToken)
    {
        // AddToSet anade sobre el estado real del documento y no duplica: dos procesos concurrentes anaden
        // cada uno el suyo y ambos sobreviven. Reescribir el documento entero hacia que el ultimo ganara.
        var update = Builders<UserAggregate>.Update
            .AddToSet(RolesDeLaCopropiedad, role)
            .Set(x => x.UpdatedBy, updatedBy)
            .Set(x => x.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        return ApplyAsync(id, tenantId, update, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<RoleAssignmentResult> RemoveRoleAsync(Guid id, Guid tenantId, Guid role, Guid updatedBy, CancellationToken cancellationToken)
    {
        var update = Builders<UserAggregate>.Update
            .Pull(RolesDeLaCopropiedad, role)
            .Set(x => x.UpdatedBy, updatedBy)
            .Set(x => x.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        return ApplyAsync(id, tenantId, update, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AddTenantAsync(Guid id, TenantEntity tenant, Guid updatedBy, CancellationToken cancellationToken)
    {
        // Se filtra por el identificador de la copropiedad, no por el objeto entero: si le cambiaran el
        // nombre, comparar el objeto completo la anadiria dos veces.
        var filter = Builders<UserAggregate>.Filter.Eq(x => x.Id, id)
            & Builders<UserAggregate>.Filter.Not(
                Builders<UserAggregate>.Filter.ElemMatch(x => x.Tenants, t => t.Id == tenant.Id));

        var update = Builders<UserAggregate>.Update
            .Push(x => x.Tenants, tenant)
            .Set(x => x.UpdatedBy, updatedBy)
            .Set(x => x.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        var result = await GetCollection<UserAggregate>()
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// El operador posicional apunta a la copropiedad que casa con el filtro, asi que la escritura entra
    /// en la correcta sin tener que saber su indice.
    /// </summary>
    private const string RolesDeLaCopropiedad = "Tenants.$.Roles";

    private async Task<RoleAssignmentResult> ApplyAsync(Guid id, Guid tenantId, UpdateDefinition<UserAggregate> update, CancellationToken cancellationToken)
    {
        // La pertenencia va dentro del filtro: si el usuario no esta en esa copropiedad no casa ningun
        // documento, y eso se distingue de que el rol ya estuviera. Comprobarla antes con una lectura
        // dejaria una ventana entre la comprobacion y la escritura.
        var filter = Builders<UserAggregate>.Filter.Eq(x => x.Id, id)
            & Builders<UserAggregate>.Filter.ElemMatch(x => x.Tenants, t => t.Id == tenantId);

        var result = await GetCollection<UserAggregate>()
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
            return RoleAssignmentResult.TenantNotFound;

        // ModifiedCount en cero significa que no habia nada que cambiar: no es un fallo.
        return result.ModifiedCount > 0 ? RoleAssignmentResult.Applied : RoleAssignmentResult.NothingToDo;
    }
}
