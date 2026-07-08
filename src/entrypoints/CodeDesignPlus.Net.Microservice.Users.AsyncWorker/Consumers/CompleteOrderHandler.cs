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

        var addRoleTask = mediator.Send(new AddRoleCommand(data.BuyerId, DefaultRole, data.BuyerId), token);

        var addTenantTask = mediator.Send(new AddTenantCommand(data.BuyerId, new TenantDto
        {
            Id = data.TenantDetail.Id,
            Name = data.TenantDetail.Name,
        }), token);

        await Task.WhenAll(addRoleTask, addTenantTask);

        var provisionedEvent = UserProvisionedForOrderDomainEvent.Create(
            data.BuyerId,
            data.AggregateId
        );

        await pubsub.PublishAsync(provisionedEvent, token);
    }
}
