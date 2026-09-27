using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemovePurgedTenant;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.RemovePurgedTenant;

public class RemovePurgedTenantCommandHandlerTest
{
    private static readonly Guid Purged = Guid.NewGuid();
    private static readonly Guid Administrator = Guid.NewGuid();

    private readonly Mock<IUserRepository> repositoryMock = new();
    private readonly Mock<IPubSub> pubSubMock = new();
    private readonly Mock<ICacheManager> cacheManagerMock = new();
    private readonly List<string> calls = [];
    private readonly RemovePurgedTenantCommandHandler handler;

    public RemovePurgedTenantCommandHandlerTest()
    {
        pubSubMock
            .Setup(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("publish"))
            .Returns(Task.CompletedTask);

        repositoryMock
            .Setup(r => r.RemoveTenantAsync(It.IsAny<Guid>(), Purged, It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("remove"))
            .ReturnsAsync(true);

        handler = new RemovePurgedTenantCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);
    }

    private static UserAggregate Member()
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);

        user.AddTenant(Purged, "Malpelo XXI", Guid.NewGuid());
        user.AddRole(Purged, Administrator, Guid.NewGuid());
        user.GetAndClearEvents();

        return user;
    }

    [Fact]
    public async Task Handle_Members_PublishesTheRoleRemovalsBeforeRemovingTheTenant()
    {
        // Arrange
        var member = Member();

        repositoryMock.Setup(r => r.FindByTenantAsync(Purged, It.IsAny<CancellationToken>())).ReturnsAsync([member]);

        // Act
        await handler.Handle(new RemovePurgedTenantCommand(Purged), CancellationToken.None);

        // Assert
        Assert.Equal(["publish", "remove"], calls);
        pubSubMock.Verify(p => p.PublishAsync(It.Is<IReadOnlyList<IDomainEvent>>(e => e.OfType<RoleRemovedToUserDomainEvent>().Single().Role == Administrator), It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.RemoveTenantAsync(member.Id, Purged, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PublishFails_KeepsTheMembershipForTheRetry()
    {
        // Arrange
        repositoryMock.Setup(r => r.FindByTenantAsync(Purged, It.IsAny<CancellationToken>())).ReturnsAsync([Member()]);

        pubSubMock
            .Setup(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service Bus is down"));

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new RemovePurgedTenantCommand(Purged), CancellationToken.None));

        // Assert
        repositoryMock.Verify(r => r.RemoveTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoMembers_DoesNothing()
    {
        // Arrange
        repositoryMock.Setup(r => r.FindByTenantAsync(Purged, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act
        await handler.Handle(new RemovePurgedTenantCommand(Purged), CancellationToken.None);

        // Assert
        Assert.Empty(calls);
    }
}
