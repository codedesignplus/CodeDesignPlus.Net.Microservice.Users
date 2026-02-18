namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;

public class AddTenantCommandHandler(IUserRepository repository, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<AddTenantCommand>
{
    public async Task Handle(AddTenantCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UserAggregate>(request.UserId, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.AddTenant(request.Tenant.Id, request.Tenant.Name, request.UserId);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        var exist = await cacheManager.ExistsAsync(request.UserId.ToString());

        if (exist)
            await cacheManager.RemoveAsync(request.UserId.ToString());
    }
}