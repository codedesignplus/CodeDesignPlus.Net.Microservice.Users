using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    /// <inheritdoc/>
    public async Task<bool> AddRoleAsync(Guid id, string role, Guid updatedBy, CancellationToken cancellationToken)
    {
        var filter = Builders<UserAggregate>.Filter.Eq(x => x.Id, id);

        // AddToSet anade sobre el estado real del documento y no duplica: dos procesos concurrentes anaden
        // cada uno el suyo y ambos sobreviven. Reescribir el documento entero hacia que el ultimo ganara.
        var update = Builders<UserAggregate>.Update
            .AddToSet(x => x.Roles, role)
            .Set(x => x.UpdatedBy, updatedBy)
            .Set(x => x.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        var result = await GetCollection<UserAggregate>()
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        // ModifiedCount en cero significa que el rol ya estaba: no es un fallo, es que no habia nada que hacer.
        return result.ModifiedCount > 0;
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
}
