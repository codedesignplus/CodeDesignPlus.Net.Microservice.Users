using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.SetTenantPurgeAfter;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.SetTenantPurgeAfter;

public class SetTenantPurgeAfterCommandHandlerTest
{
    private static readonly Guid Tenant = Guid.NewGuid();

    private readonly Mock<IUserRepository> repositoryMock = new();
    private readonly Mock<ICacheManager> cacheManagerMock = new();
    private readonly Mock<IPubSub> pubSubMock = new();
    private readonly SetTenantPurgeAfterCommandHandler handler;

    public SetTenantPurgeAfterCommandHandlerTest()
    {
        handler = new SetTenantPurgeAfterCommandHandler(repositoryMock.Object, cacheManagerMock.Object);
    }

    private static UserAggregate Member()
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);

        user.AddTenant(Tenant, "Malpelo XXI", Guid.NewGuid());

        return user;
    }

    [Fact]
    public async Task Handle_TenantDeleted_MarksEveryMemberAndClearsTheirCache()
    {
        // Arrange
        var first = Member();
        var second = Member();
        var purgeAfter = SystemClock.Instance.GetCurrentInstant() + Duration.FromDays(30);

        repositoryMock.Setup(r => r.FindByTenantAsync(Tenant, It.IsAny<CancellationToken>())).ReturnsAsync([first, second]);
        cacheManagerMock.Setup(c => c.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        await handler.Handle(new SetTenantPurgeAfterCommand(Tenant, purgeAfter), CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SetTenantPurgeAfterAsync(first.Id, Tenant, purgeAfter, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.SetTenantPurgeAfterAsync(second.Id, Tenant, purgeAfter, It.IsAny<CancellationToken>()), Times.Once);
        cacheManagerMock.Verify(c => c.RemoveAsync(first.Id.ToString()), Times.Once);
        cacheManagerMock.Verify(c => c.RemoveAsync(second.Id.ToString()), Times.Once);
    }

    [Fact]
    public async Task Handle_TenantRestored_ClearsTheMarkWithoutTouchingRoles()
    {
        // Arrange
        var member = Member();

        repositoryMock.Setup(r => r.FindByTenantAsync(Tenant, It.IsAny<CancellationToken>())).ReturnsAsync([member]);

        // Act
        await handler.Handle(new SetTenantPurgeAfterCommand(Tenant, null), CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.SetTenantPurgeAfterAsync(member.Id, Tenant, null, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(r => r.RemoveRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        repositoryMock.Verify(r => r.RemoveTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
