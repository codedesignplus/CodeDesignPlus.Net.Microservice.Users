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
        var request = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@fake.com", "1234567890", "1234567890", null, true);
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
        var request = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@fake.com", "1234567890", "1234567890", null, true);

        var cancellationToken = CancellationToken.None;

        repositoryMock
            .Setup(repo => repo.ExistsAsync<UserAggregate>(request.Id, cancellationToken))
            .ReturnsAsync(false);

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserAggregate>(), cancellationToken), Times.Once);
    }

    /// <summary>
    /// Un correo, una cuenta: invitar otra vez a la misma persona no crea un segundo usuario (pendings/215).
    /// </summary>
    [Fact]
    public async Task Handle_EmailAlreadyTaken_ThrowsUserEmailAlreadyExists()
    {
        // Arrange
        var request = new CreateUserCommand(Guid.NewGuid(), "Jhon Fredy", "Castaño Ríos", "Jhon Fredy Castaño Ríos", " Porteria1@Fake.com ", "3105550131", "1023456781", null, true);

        repositoryMock.Setup(repo => repo.ExistsAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        repositoryMock.Setup(repo => repo.ExistsByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.Equal(Errors.UserEmailAlreadyExists.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
        repositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
