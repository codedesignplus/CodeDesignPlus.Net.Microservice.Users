using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.SetTenantPurgeAfter;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

/// <summary>
/// Al restaurarse una copropiedad, se le quita la marca en sus miembros: vuelve a aparecer en <c>/init</c>.
/// </summary>
[QueueName<UserAggregate>("UnmarkRestoredTenantHandler")]
public class UnmarkRestoredTenantHandler(IMediator mediator) : IEventHandler<TenantRestoredDomainEvent>
{
    public Task HandleAsync(TenantRestoredDomainEvent data, CancellationToken token)
    {
        return mediator.Send(new SetTenantPurgeAfterCommand(data.AggregateId, null), token);
    }
}
