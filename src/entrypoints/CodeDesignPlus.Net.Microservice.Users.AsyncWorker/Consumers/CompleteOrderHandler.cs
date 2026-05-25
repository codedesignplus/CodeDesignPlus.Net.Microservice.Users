using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

[QueueName<UserAggregate>("CompleteOrderHandler")]
public class CompleteOrderHandler(IMediator mediator, ILogger<CompleteOrderHandler> logger) : IEventHandler<OrderPaidAndReadyForProvisioningDomainEvent>
{
    private const string DefaultRole = "Administrador";

    public async Task HandleAsync(OrderPaidAndReadyForProvisioningDomainEvent data, CancellationToken token)
    {
        try
        {
            var addRoleTask = mediator.Send(new AddRoleCommand(data.BuyerId, DefaultRole, data.BuyerId), token);

            var addTenantTask = mediator.Send(new AddTenantCommand(data.BuyerId, new TenantDto
            {
                Id = data.TenantDetail.Id,
                Name = data.TenantDetail.Name,
            }), token);

            await Task.WhenAll(addRoleTask, addTenantTask);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process {EventName} for buyer {BuyerId}. Role or tenant may already be assigned. Skipping.", nameof(OrderPaidAndReadyForProvisioningDomainEvent), data.BuyerId);
        }
    }
}
