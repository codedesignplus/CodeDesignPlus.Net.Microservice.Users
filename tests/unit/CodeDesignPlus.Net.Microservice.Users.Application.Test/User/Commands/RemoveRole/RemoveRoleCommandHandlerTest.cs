using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.RemoveRole;

public class RemoveRoleCommandHandlerTest
{
    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

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
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new RemoveRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_RemovesRoleAndPublishesEvents()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        var command = new RemoveRoleCommand(aggregate.Id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        repositoryMock.Setup(r => r.RemoveRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(RoleAssignmentResult.Applied);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert: se retira con una escritura atomica, no reescribiendo el documento entero, por la misma
        // razon que en la asignacion.
        repositoryMock.Verify(r => r.RemoveRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TenantNotFound_ThrowsAndPublishesNothing()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        var command = new RemoveRoleCommand(aggregate.Id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        repositoryMock.Setup(r => r.RemoveRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(RoleAssignmentResult.TenantNotFound);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.TenantNotFound.GetCode(), exception.Code);
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RoleWasNotThere_PublishesNothing()
    {
        // Arrange: los consumidores reintentan, asi que la misma revocacion puede llegar mas de una vez.
        // Anunciarla de nuevo haria creer que acaba de pasar algo que ya habia pasado.
        var repositoryMock = new Mock<IUserRepository>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new RemoveRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        var command = new RemoveRoleCommand(aggregate.Id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        repositoryMock.Setup(r => r.RemoveRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(RoleAssignmentResult.NothingToDo);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
