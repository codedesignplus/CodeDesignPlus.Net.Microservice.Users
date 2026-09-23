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
        var handler = new AddRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

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
        var handler = new AddRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

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
        var handler = new AddRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var aggregate = UserAggregate.Create(command.Id, "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());

        repositoryMock
            .Setup(repo => repo.AddRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RoleAssignmentResult.Applied);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        // La escritura la hace la base con AddToSet, no un reemplazo del documento entero: asi dos
        // asignaciones simultaneas no se pisan.
        repositoryMock.Verify(repo => repo.AddRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
        pubSubMock.Verify(pub => pub.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TenantNotFound_ThrowsAndPublishesNothing()
    {
        // Arrange: un rol cuelga de una copropiedad. Si el usuario no pertenece a ella, no hay papel que
        // darle, y publicar el evento haria que ms-microsoftgraph lo metiera igual en el grupo global.
        var repositoryMock = new Mock<IUserRepository>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new AddRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var aggregate = UserAggregate.Create(command.Id, "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        repositoryMock
            .Setup(repo => repo.AddRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RoleAssignmentResult.TenantNotFound);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.TenantNotFound.GetCode(), exception.Code);
        pubSubMock.Verify(pub => pub.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RoleAlreadyThere_PublishesNothing()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var pubSubMock = new Mock<IPubSub>();
        var cacheManagerMock = new Mock<ICacheManager>();
        var handler = new AddRoleCommandHandler(repositoryMock.Object, pubSubMock.Object, cacheManagerMock.Object);

        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var aggregate = UserAggregate.Create(command.Id, "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        repositoryMock
            .Setup(repo => repo.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        repositoryMock
            .Setup(repo => repo.AddRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(RoleAssignmentResult.NothingToDo);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert: repetir la operacion es inofensivo y no vuelve a avisar a nadie.
        pubSubMock.Verify(pub => pub.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
