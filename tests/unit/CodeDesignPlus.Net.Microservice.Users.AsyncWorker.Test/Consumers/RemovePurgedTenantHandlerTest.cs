using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemovePurgedTenant;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Test.Consumers;

public class RemovePurgedTenantHandlerTest
{
    [Fact]
    public async Task HandleAsync_TenantPurged_RemovesItFromItsMembers()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var tenant = Guid.NewGuid();
        var handler = new RemovePurgedTenantHandler(mediator.Object);

        // Act
        await handler.HandleAsync(TenantPurgedDomainEvent.Create(tenant, "Malpelo XXI"), CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<RemovePurgedTenantCommand>(c => c.TenantId == tenant), It.IsAny<CancellationToken>()), Times.Once);
    }
}
