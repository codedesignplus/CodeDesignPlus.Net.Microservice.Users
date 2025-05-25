using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateProfile;

public class UpdateProfileCommandHandlerTest
{
    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var handler = new UpdateProfileCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

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
        var handler = new UpdateProfileCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "JD", "joe@fake.com", "1234567890", true, ContactInfo.Create("Cll 3", "City", "State", "Country", "12345", "+57 3107565142", ["joe@fake.com"]), JobInfo.Create("Developer", "Microsoft", "IT", "1234567890", "Custom", SystemClock.Instance.GetCurrentInstant(), "Office Location"));
        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesUserAndPublishesEvents()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var handler = new UpdateProfileCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "JD", "joe@fake.com", "1234567890", true, ContactInfo.Create("Cll 3", "City", "State", "Country", "12345", "+57 3107565142", ["joe@fake.com"]), JobInfo.Create("Developer", "Microsoft", "IT", "1234567890", "Custom", SystemClock.Instance.GetCurrentInstant(), "Office Location"));

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.UpdateAsync(aggregate, It.IsAny<CancellationToken>()), Times.Once);
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<List<ProfileUpdatedDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }
}
