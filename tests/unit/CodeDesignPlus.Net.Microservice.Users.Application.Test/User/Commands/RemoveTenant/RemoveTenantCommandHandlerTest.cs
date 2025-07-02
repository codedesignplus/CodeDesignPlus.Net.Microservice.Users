using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveTenant;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.RemoveTenant;

public class RemoveTenantCommandHandlerTest
{
    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveTenantCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(null!, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveTenantCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new RemoveTenantCommand(Guid.NewGuid(), Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));
        
        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesRepositoryAndPublishesEvents()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveTenantCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true);
        var command = new RemoveTenantCommand(aggregate.Id, Guid.NewGuid());

        aggregate.AddTenant(command.IdTenant, "TestTenant", Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.UpdateAsync(aggregate, It.IsAny<CancellationToken>()), Times.Once);
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<List<TenantRemovedDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }
}
