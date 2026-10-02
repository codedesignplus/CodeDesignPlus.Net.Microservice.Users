using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.RemoveRole;

/// <summary>
/// Al retirar el último rol de una copropiedad, el usuario deja de pertenecer a ella; y si ya no le queda ninguna
/// copropiedad ni rol de plataforma, se borra (pendings/211).
/// </summary>
public class RemoveRoleReleasesTheUserTest
{
    private static readonly Guid Tenant = Guid.NewGuid();
    private static readonly Guid OtherTenant = Guid.NewGuid();
    private static readonly Guid Role = Guid.NewGuid();
    private static readonly Guid OtherRole = Guid.NewGuid();
    private static readonly Guid Admin = Guid.NewGuid();

    /// <summary>
    /// El usuario tal como queda en la base tras la escritura atómica que retira el rol.
    /// </summary>
    private static UserAggregate After(params (Guid Tenant, Guid[] Roles)[] tenants)
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "Jhon Fredy", "Castaño Ríos", "porteria@fake.com", "3105550131", "Jhon Fredy Castaño Ríos", "1023456781", null, true);

        foreach (var (tenant, roles) in tenants)
        {
            user.AddTenant(tenant, "Cuatro Vientos", Admin);

            foreach (var role in roles)
                user.AddRole(tenant, role, Admin);
        }

        user.GetAndClearEvents();
        return user;
    }

    private static (RemoveRoleCommandHandler Handler, Mock<IUserRepository> Repository, List<IDomainEvent> Published) Arrange(UserAggregate after)
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.FindAsync<UserAggregate>(after.Id, It.IsAny<CancellationToken>())).ReturnsAsync(after);
        repository.Setup(r => r.RemoveRoleAsync(after.Id, Tenant, Role, Admin, It.IsAny<CancellationToken>())).ReturnsAsync(RoleAssignmentResult.Applied);

        var published = new List<IDomainEvent>();
        var pubsub = new Mock<IPubSub>();
        pubsub.Setup(p => p.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyList<IDomainEvent>, CancellationToken>((events, _) => published.AddRange(events))
            .Returns(Task.CompletedTask);

        return (new RemoveRoleCommandHandler(repository.Object, pubsub.Object, Mock.Of<ICacheManager>()), repository, published);
    }

    [Fact]
    public async Task Handle_LastRoleOfTheOnlyTenant_DeletesTheUser()
    {
        // Arrange
        var after = After((Tenant, []));
        var (handler, repository, published) = Arrange(after);

        // Act
        await handler.Handle(new RemoveRoleCommand(after.Id, Tenant, Role, Admin), CancellationToken.None);

        // Assert
        repository.Verify(r => r.DeleteAsync<UserAggregate>(after.Id, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Contains(published, e => e is TenantRemovedDomainEvent);
        Assert.Contains(published, e => e is UserDeletedDomainEvent);
    }

    [Fact]
    public async Task Handle_LastRoleButAnotherTenantLeft_RemovesOnlyThatTenant()
    {
        // Arrange
        var after = After((Tenant, []), (OtherTenant, [OtherRole]));
        var (handler, repository, published) = Arrange(after);

        // Act
        await handler.Handle(new RemoveRoleCommand(after.Id, Tenant, Role, Admin), CancellationToken.None);

        // Assert
        Assert.DoesNotContain(after.Tenants, t => t.Id == Tenant);
        repository.Verify(r => r.UpdateAsync(after, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.DeleteAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.DoesNotContain(published, e => e is UserDeletedDomainEvent);
    }

    [Fact]
    public async Task Handle_AnotherRoleLeftInTheTenant_KeepsTheUserAsIs()
    {
        // Arrange
        var after = After((Tenant, [OtherRole]));
        var (handler, repository, published) = Arrange(after);

        // Act
        await handler.Handle(new RemoveRoleCommand(after.Id, Tenant, Role, Admin), CancellationToken.None);

        // Assert
        Assert.Single(after.Tenants);
        repository.Verify(r => r.UpdateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.DeleteAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal([typeof(RoleRemovedToUserDomainEvent)], published.Select(e => e.GetType()));
    }
}
