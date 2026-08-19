using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using FailedEvent = CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents.UserProvisioningFailedForOrderDomainEvent;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

[QueueName<UserAggregate>("CompleteOrderHandler")]
public class CompleteOrderHandler(IMediator mediator, IUserRepository userRepository, IPubSub pubsub, ILogger<CompleteOrderHandler> logger) : IEventHandler<OrderPaidAndReadyForProvisioningDomainEvent>
{
    private const string DefaultRole = "Administrador";

    public async Task HandleAsync(OrderPaidAndReadyForProvisioningDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<UserAggregate>(data.BuyerId, token);

        if (!exists)
        {
            logger.LogWarning("User {Id} not found. Publishing provisioning failure for Order {OrderId}.", data.BuyerId, data.AggregateId);

            var failedEvent = FailedEvent.Create(
                data.BuyerId,
                data.AggregateId,
                $"User {data.BuyerId} not found in ms-users"
            );

            await pubsub.PublishAsync(failedEvent, token);
            return;
        }

        // Uno detras de otro, no en paralelo. Los dos comandos leen el mismo usuario, cambian una lista
        // distinta y reescriben el documento entero: lanzados a la vez leen el mismo estado de partida y el
        // ultimo en guardar borra lo del otro. Ocurrio en una compra real — el rol quedo grabado y la
        // copropiedad no, asi que el comprador no veia nada al entrar pese a que ambos eventos se publicaron.
        await mediator.Send(new AddRoleCommand(data.BuyerId, DefaultRole, data.BuyerId), token);

        await mediator.Send(new AddTenantCommand(data.BuyerId, new TenantDto
        {
            Id = data.TenantDetail.Id,
            Name = data.TenantDetail.Name,
        }), token);

        var provisionedEvent = UserProvisionedForOrderDomainEvent.Create(
            data.BuyerId,
            data.AggregateId
        );

        await pubsub.PublishAsync(provisionedEvent, token);
    }
}
