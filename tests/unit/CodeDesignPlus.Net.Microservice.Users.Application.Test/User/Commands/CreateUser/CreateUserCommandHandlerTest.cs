using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.CreateUser;

public class CreateUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IPubSub> pubSubMock;
    private readonly CreateUserCommandHandler handler;

    public CreateUserCommandHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        pubSubMock = new Mock<IPubSub>();

        handler = new CreateUserCommandHandler(repositoryMock.Object,  pubSubMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        CreateUserCommand request = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserAlreadyExists_ThrowsUserAlreadyExistsException()
    {
        // Arrange
        var request = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@fake.com", "1234567890", true);
        var cancellationToken = CancellationToken.None;

        repositoryMock
            .Setup(repo => repo.ExistsAsync<UserAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, cancellationToken));

        Assert.Equal(Errors.UserAlreadyExists.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserAlreadyExists.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUserAndPublishesEvents()
    {
        // Arrange
        var request = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@fake.com", "1234567890", true);

        var cancellationToken = CancellationToken.None;

        repositoryMock
            .Setup(repo => repo.ExistsAsync<UserAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(false);

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserAggregate>(), cancellationToken), Times.Once);
    }
}
