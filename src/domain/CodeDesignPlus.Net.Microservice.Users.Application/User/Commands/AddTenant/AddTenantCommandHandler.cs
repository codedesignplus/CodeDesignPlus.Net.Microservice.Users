using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;

public class AddTenantCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<AddTenantCommand>
{
    public async Task Handle(AddTenantCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.UserId, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        var tenant = new TenantEntity { Id = request.Tenant.Id, Name = request.Tenant.Name };

        var added = await repository.AddTenantAsync(request.UserId, tenant, request.UserId, cancellationToken);

        if (!added)
            return;

        await pubsub.PublishAsync(
            [TenantAddedDomainEvent.Create(aggregate.Id, aggregate.DisplayName, aggregate.Email, tenant)],
            cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.UserId.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.UserId.ToString());
    }
}
