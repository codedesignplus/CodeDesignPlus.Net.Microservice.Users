using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.SetTenantPurgeAfter;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

/// <summary>
/// Al darse de baja una copropiedad, se marca en sus miembros para que <c>/init</c> no la ofrezca. Nada se quita:
/// restaurarla se la devuelve a todos.
/// </summary>
[QueueName<UserAggregate>("MarkDeletedTenantHandler")]
public class MarkDeletedTenantHandler(IMediator mediator) : IEventHandler<TenantDeletedDomainEvent>
{
    public Task HandleAsync(TenantDeletedDomainEvent data, CancellationToken token)
    {
        // Sin fecha, el evento viene de un ms-tenants que borraba en el acto: ya está purgada.
        var purgeAfter = data.PurgeAfter ?? data.OccurredAt;

        return mediator.Send(new SetTenantPurgeAfterCommand(data.AggregateId, purgeAfter), token);
    }
}
