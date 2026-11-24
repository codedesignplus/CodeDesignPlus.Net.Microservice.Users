using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.AddRole;

/// <summary>
/// La cuenta que el sistema desactivó por no tener nada vuelve a entrar en cuanto recibe un rol.
/// </summary>
public class AddRoleReactivatesTheReleasedAccountTest
{
    private static (AddRoleCommandHandler Handler, List<IDomainEvent> Published, AddRoleCommand Command) Arrange(bool released)
    {
        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var user = UserAggregate.Create(command.Id, "Laura", "Gómez Ríos", "laura@fake.com", "3105550131", null, "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), true, DocumentCheckDigitRules.None);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.FindAsync<UserAggregate>(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repository.Setup(r => r.AddRoleAsync(command.Id, command.TenantId, command.Role, command.IdUser, It.IsAny<CancellationToken>())).ReturnsAsync(RoleAssignmentResult.Applied);
        repository.Setup(r => r.ReactivateReleasedAsync(command.Id, command.IdUser, It.IsAny<CancellationToken>())).ReturnsAsync(released);

        var published = new List<IDomainEvent>();
        var pubsub = new Mock<IPubSub>();
        pubsub.Setup(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyList<IDomainEvent>, CancellationToken>((events, _) => published.AddRange(events))
            .Returns(Task.CompletedTask);

        return (new AddRoleCommandHandler(repository.Object, Mock.Of<ITenantDirectory>(), pubsub.Object, Mock.Of<ICacheManager>()), published, command);
    }

    [Fact]
    public async Task Handle_ReleasedAccount_AnnouncesTheReactivation()
    {
        // Arrange
        var (handler, published, command) = Arrange(released: true);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal([typeof(RoleAddedToUserDomainEvent), typeof(UserReactivatedDomainEvent)], published.Select(e => e.GetType()));
    }

    [Fact]
    public async Task Handle_ActiveAccount_AnnouncesOnlyTheRole()
    {
        // Arrange
        var (handler, published, command) = Arrange(released: false);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal([typeof(RoleAddedToUserDomainEvent)], published.Select(e => e.GetType()));
    }
}
