using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.AddRole;

public class AddRoleCommandHandlerTest
{
    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new AddRoleCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);

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
        var handler = new AddRoleCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new AddRoleCommand(Guid.NewGuid(), "Admin");

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsRoleAndPublishesEvents()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new AddRoleCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new AddRoleCommand(Guid.NewGuid(), "Admin");
        var aggregate = UserAggregate.Create(command.Id, "John", "Doe", "john@fake.com", "1234567890", "JD", true);

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(repo => repo.UpdateAsync(aggregate, It.IsAny<CancellationToken>()), Times.Once);
        pubSubMock.Verify(pub => pub.PublishAsync(It.IsAny<List<RoleAddedToUserDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }
}
