using NodaTime;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using CodeDesignPlus.Net.Microservice.Users.Infrastructure.Test.Helpers;
using CodeDesignPlus.Net.Mongo.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Users.Infrastructure.Test.Repositories;

/// <summary>
/// Retirar una copropiedad purgada, contra un Mongo real: el <c>$pull</c> tiene que quitar solo esa copropiedad y
/// dejar intactas las demas del usuario.
/// </summary>
[Collection(MongoContainerFixture.Collection)]
public class UserRepositoryTest
{
    private readonly IUserRepository repository;

    public UserRepositoryTest(MongoContainerFixture fixture)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Mongo:Enable"] = "true",
                ["Mongo:ConnectionString"] = fixture.ConnectionString,
                ["Mongo:Database"] = $"db-ms-users-{Guid.NewGuid():N}",
                ["Mongo:RegisterHealthCheck"] = "false",
            })
            .Build();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddMongo<Startup>(configuration);

        this.repository = services.BuildServiceProvider().GetRequiredService<IUserRepository>();
    }

    private async Task<UserAggregate> MemberAsync(params Guid[] tenants)
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", $"{Guid.NewGuid():N}@example.com", "3107845123", null, "1234567890", null, true);

        foreach (var tenant in tenants)
        {
            user.AddTenant(tenant, "Malpelo", Guid.NewGuid());
            user.AddRole(tenant, Guid.NewGuid(), Guid.NewGuid());
        }

        await repository.CreateAsync(user, CancellationToken.None);

        return user;
    }

    [Fact]
    public async Task FindByTenantAsync_OnlyItsMembers_IncludingDeletedUsers()
    {
        // Arrange
        var purged = Guid.NewGuid();
        var member = await MemberAsync(purged);
        var outsider = await MemberAsync(Guid.NewGuid());
        var deleted = await MemberAsync(purged);

        deleted.Delete(Guid.NewGuid());

        await repository.UpdateAsync(deleted, CancellationToken.None);

        // Act
        var members = await repository.FindByTenantAsync(purged, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { member.Id, deleted.Id }.Order(), members.Select(x => x.Id).Order());
        Assert.DoesNotContain(members, x => x.Id == outsider.Id);
    }

    [Fact]
    public async Task RemoveTenantAsync_Member_RemovesOnlyThatTenant()
    {
        // Arrange
        var purged = Guid.NewGuid();
        var kept = Guid.NewGuid();
        var member = await MemberAsync(purged, kept);

        // Act
        var removed = await repository.RemoveTenantAsync(member.Id, purged, CancellationToken.None);

        // Assert
        var after = await repository.FindAsync<UserAggregate>(member.Id, CancellationToken.None);

        Assert.True(removed);
        Assert.Equal([kept], after.Tenants.Select(x => x.Id));
        Assert.Single(after.Tenants.Single().Roles);
    }

    [Fact]
    public async Task SetTenantPurgeAfterAsync_MarkAndUnmark_TouchesOnlyThatTenant()
    {
        // Arrange
        var deleted = Guid.NewGuid();
        var kept = Guid.NewGuid();
        var member = await MemberAsync(deleted, kept);
        var purgeAfter = Instant.FromUnixTimeMilliseconds(SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()) + Duration.FromDays(30);

        // Act
        var marked = await repository.SetTenantPurgeAfterAsync(member.Id, deleted, purgeAfter, CancellationToken.None);
        var afterMark = await repository.FindAsync<UserAggregate>(member.Id, CancellationToken.None);

        await repository.SetTenantPurgeAfterAsync(member.Id, deleted, null, CancellationToken.None);
        var afterUnmark = await repository.FindAsync<UserAggregate>(member.Id, CancellationToken.None);

        // Assert
        Assert.True(marked);
        Assert.Equal(purgeAfter, afterMark.Tenants.Single(x => x.Id == deleted).PurgeAfter);
        Assert.Null(afterMark.Tenants.Single(x => x.Id == kept).PurgeAfter);
        Assert.Single(afterMark.Tenants.Single(x => x.Id == deleted).Roles);
        Assert.Null(afterUnmark.Tenants.Single(x => x.Id == deleted).PurgeAfter);
    }

    [Fact]
    public async Task RemoveTenantAsync_NotAMember_ReturnsFalse()
    {
        // Arrange
        var member = await MemberAsync(Guid.NewGuid());

        // Act
        var removed = await repository.RemoveTenantAsync(member.Id, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(removed);
    }
}
