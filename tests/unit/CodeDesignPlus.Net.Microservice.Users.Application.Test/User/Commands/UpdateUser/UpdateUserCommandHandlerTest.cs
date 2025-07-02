using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateUser;

public class UpdateUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IUserContext> userContextMock;
    private readonly Mock<IPubSub> pubSubMock;
    private readonly UpdateUserCommandHandler handler;

    public UpdateUserCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        userContextMock = new Mock<IUserContext>();
        pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        handler = new UpdateUserCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object, cacheManagerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        UpdateUserCommand request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));
        
        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var request = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john@fake.com", "1234567890", true);
        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exceptino = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exceptino.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exceptino.Code);
        Assert.Equal(Layer.Application, exceptino.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesUserAndPublishesEvents()
    {
        // Arrange
        var request = new UpdateUserCommand(Guid.NewGuid(), "John U", "Doe U", "JDU", "john@fake.com", "1234567890", true);

        var aggregate = UserAggregate.Create(request.Id, "John", "Doe", "john@fake.com", "1234567890", "JD", true);

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        repositoryMock.Verify(repo => repo.UpdateAsync(aggregate, It.IsAny<CancellationToken>()), Times.Once);
        pubSubMock.Verify(pubsub => pubsub.PublishAsync(It.IsAny<List<UserUpdatedDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }
}
