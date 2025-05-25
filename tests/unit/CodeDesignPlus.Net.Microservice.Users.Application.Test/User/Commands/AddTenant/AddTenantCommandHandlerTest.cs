using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.AddTenant;

public class AddTenantCommandHandlerTest
{
    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();

        var handler = new AddTenantCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

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

        repositoryMock
            .Setup(r => r.FindAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAggregate)null!);

        var handler = new AddTenantCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        var command = new AddTenantCommand(Guid.NewGuid(), new TenantDto { Id = Guid.NewGuid(), Name = "TestTenant" });

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

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());

        var handler = new AddTenantCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        var command = new AddTenantCommand(aggregate.Id, new TenantDto { Id = Guid.NewGuid(), Name = "TestTenant" });

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.UpdateAsync(aggregate, It.IsAny<CancellationToken>()), Times.Once);
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<List<TenantAddedDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }
}
