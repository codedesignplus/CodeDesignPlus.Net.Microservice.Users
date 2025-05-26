using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.DeleteUser;

public class DeleteUserCommandHandlerTest
{
    [Fact]
    public async Task Handle_NullRequest_ThrowsInvalidRequestException()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        var mockUserContext = new Mock<IUserContext>();
        var mockPubSub = new Mock<IPubSub>();

        var handler = new DeleteUsersCommandHandler(mockRepository.Object, mockUserContext.Object, mockPubSub.Object);

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
        var mockRepository = new Mock<IUserRepository>();
        var mockUserContext = new Mock<IUserContext>();
        var mockPubSub = new Mock<IPubSub>();
        var command = new DeleteUserCommand(Guid.NewGuid());

        mockRepository
            .Setup(x => x.FindAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAggregate)null!);

        var handler = new DeleteUsersCommandHandler(mockRepository.Object, mockUserContext.Object, mockPubSub.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesUserAndPublishesEvents()
    {
        // Arrange
        var mockRepository = new Mock<IUserRepository>();
        var mockUserContext = new Mock<IUserContext>();
        var mockPubSub = new Mock<IPubSub>();
        var userId = Guid.NewGuid();
        var aggregate = UserAggregate.Create(userId, "John", "Doe", "john@fake.com", "123456789", "John Doe", true);
        var command = new DeleteUserCommand(userId);

        mockUserContext.Setup(x => x.IdUser).Returns(Guid.NewGuid());
        mockRepository.Setup(x => x.FindAsync<UserAggregate>(userId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        var handler = new DeleteUsersCommandHandler(mockRepository.Object, mockUserContext.Object, mockPubSub.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(x => x.DeleteAsync<UserAggregate>(userId, It.IsAny<CancellationToken>()), Times.Once);
        mockPubSub.Verify(x => x.PublishAsync(It.IsAny<List<UserDeletedDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }

}
