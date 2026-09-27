using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemovePurgedTenant;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

/// <summary>
/// Al purgarse una copropiedad, se la quita a todos sus miembros con sus roles. Las cuentas se conservan.
/// </summary>
[QueueName<UserAggregate>("RemovePurgedTenantHandler")]
public class RemovePurgedTenantHandler(IMediator mediator) : IEventHandler<TenantPurgedDomainEvent>
{
    public Task HandleAsync(TenantPurgedDomainEvent data, CancellationToken token)
    {
        return mediator.Send(new RemovePurgedTenantCommand(data.AggregateId), token);
    }
}
