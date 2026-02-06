using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

[QueueName<UserAggregate>("CompleteOrderHandler")]
public class CompleteOrderHandler(IMediator mediator) : IEventHandler<OrderPaidAndReadyForProvisioningDomainEvent>
{
    private const string DefaultRole = "Administrador";

    public Task HandleAsync(OrderPaidAndReadyForProvisioningDomainEvent data, CancellationToken token)
    {
        var addRoleTask = mediator.Send(new AddRoleCommand(data.BuyerId, DefaultRole), token);

        var addTenantTask = mediator.Send(new AddTenantCommand(data.BuyerId, new TenantDto
        {
            Id = data.TenantDetail.Id,
            Name = data.TenantDetail.Name,
        }), token);


        return Task.WhenAll(addRoleTask, addTenantTask);
    }
}
