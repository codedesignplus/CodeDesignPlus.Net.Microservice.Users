using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.SetTenantPurgeAfter;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Test.Consumers;

public class TenantDeletionMarkHandlersTest
{
    private readonly Mock<IMediator> mediator = new();

    [Fact]
    public async Task MarkDeletedTenantHandler_WithPurgeDate_MarksWithThatDate()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var purgeAfter = SystemClock.Instance.GetCurrentInstant() + Duration.FromDays(30);
        var handler = new MarkDeletedTenantHandler(mediator.Object);

        // Act
        await handler.HandleAsync(TenantDeletedDomainEvent.Create(tenant, "Malpelo XXI", purgeAfter), CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<SetTenantPurgeAfterCommand>(c => c.TenantId == tenant && c.PurgeAfter == purgeAfter), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkDeletedTenantHandler_WithoutPurgeDate_MarksItAsAlreadyPurged()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var @event = TenantDeletedDomainEvent.Create(tenant, "Malpelo XXI", null);
        var handler = new MarkDeletedTenantHandler(mediator.Object);

        // Act
        await handler.HandleAsync(@event, CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<SetTenantPurgeAfterCommand>(c => c.TenantId == tenant && c.PurgeAfter == @event.OccurredAt), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UnmarkRestoredTenantHandler_TenantRestored_ClearsTheMark()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var handler = new UnmarkRestoredTenantHandler(mediator.Object);

        // Act
        await handler.HandleAsync(TenantRestoredDomainEvent.Create(tenant, "Malpelo XXI"), CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<SetTenantPurgeAfterCommand>(c => c.TenantId == tenant && c.PurgeAfter == null), It.IsAny<CancellationToken>()), Times.Once);
    }
}
