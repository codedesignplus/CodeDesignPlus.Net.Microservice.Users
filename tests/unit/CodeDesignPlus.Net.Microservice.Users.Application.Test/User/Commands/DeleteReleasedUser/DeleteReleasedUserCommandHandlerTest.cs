using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteReleasedUser;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.DeleteReleasedUser;

/// <summary>
/// Solo se borra la cuenta que el sistema desactivó por no tener nada, y solo si sigue así.
/// </summary>
public class DeleteReleasedUserCommandHandlerTest
{
    private static readonly Guid Admin = Guid.NewGuid();

    private static UserAggregate User()
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "Laura", "Gómez Ríos", "laura@fake.com", "3105550131", null, "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), true, DocumentCheckDigitRules.None);
        user.GetAndClearEvents();

        return user;
    }

    private static (DeleteReleasedUserCommandHandler Handler, Mock<IUserRepository> Repository, List<IDomainEvent> Published) Arrange(UserAggregate? user, Guid id)
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.FindAsync<UserAggregate>(id, It.IsAny<CancellationToken>())).ReturnsAsync(user!);

        var published = new List<IDomainEvent>();
        var pubsub = new Mock<IPubSub>();
        pubsub.Setup(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyList<IDomainEvent>, CancellationToken>((events, _) => published.AddRange(events))
            .Returns(Task.CompletedTask);

        return (new DeleteReleasedUserCommandHandler(repository.Object, pubsub.Object, Mock.Of<ICacheManager>()), repository, published);
    }

    [Fact]
    public async Task Handle_ReleasedAccount_DeletesItAndAnnouncesIt()
    {
        // Arrange
        var user = User();
        user.Release(Admin);
        user.GetAndClearEvents();
        var (handler, repository, published) = Arrange(user, user.Id);

        // Act
        var deleted = await handler.Handle(new DeleteReleasedUserCommand(user.Id, Admin), CancellationToken.None);

        // Assert
        repository.Verify(r => r.DeleteAsync<UserAggregate>(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal((true, true), (deleted, published.Exists(e => e is UserDeletedDomainEvent)));
    }

    [Fact]
    public async Task Handle_AccountThatJoinedATenantAgain_IsNotDeleted()
    {
        // Arrange
        var user = User();
        user.Release(Admin);
        user.AddTenant(Guid.NewGuid(), "Las Acacias", Admin);
        var (handler, repository, _) = Arrange(user, user.Id);

        // Act
        var deleted = await handler.Handle(new DeleteReleasedUserCommand(user.Id, Admin), CancellationToken.None);

        // Assert
        Assert.False(deleted);
        repository.Verify(r => r.DeleteAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ActiveAccountWithNothing_IsNotDeleted()
    {
        // Arrange
        var user = User();
        var (handler, repository, _) = Arrange(user, user.Id);

        // Act
        var deleted = await handler.Handle(new DeleteReleasedUserCommand(user.Id, Admin), CancellationToken.None);

        // Assert
        Assert.False(deleted);
        repository.Verify(r => r.DeleteAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserAlreadyGone_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var (handler, _, _) = Arrange(null, id);

        // Act
        var deleted = await handler.Handle(new DeleteReleasedUserCommand(id, Admin), CancellationToken.None);

        // Assert
        Assert.False(deleted);
    }
}
