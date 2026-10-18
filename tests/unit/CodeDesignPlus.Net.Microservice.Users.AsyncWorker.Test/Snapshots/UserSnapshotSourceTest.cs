using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Snapshots;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Test.Snapshots;

/// <summary>
/// The user snapshot a local copy asks for at startup: every live user with name, document and contact, so a copy created
/// after its users does not leave them out.
/// </summary>
public class UserSnapshotSourceTest
{
    [Fact]
    public void Key_IsTheUserAggregateSnapshot()
    {
        // Act
        var key = SnapshotKey.Of(typeof(UserSnapshotDomainEvent));

        // Assert
        Assert.Equal(("UserAggregate", "UserSnapshotDomainEvent"), (key.Aggregate, key.Snapshot));
    }

    [Fact]
    public async Task GetPageAsync_LiveUser_CarriesNameDocumentAndContactOnThePlatformTenant()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var documentType = new DocumentType(Guid.NewGuid(), "Cedula de ciudadania", "CC");
        var user = UserAggregate.Create(Guid.NewGuid(), "Andres", "Rojas", "andres@example.com", "3107845123", null, "1012345678", documentType, true);

        user.UpdateContactInfo("Calle 5 # 3-10", "Bogota", "Cundinamarca", "Colombia", "110111", "3107845123", ["andres@example.com"], Guid.NewGuid());

        var repository = new Mock<IUserRepository>();

        repository
            .Setup(x => x.GetSnapshotPageAsync(tenant, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserAggregate> { user });

        var source = new UserSnapshotSource(repository.Object);

        // Act
        var page = await source.GetPageAsync(tenant, 1, 50, CancellationToken.None);

        // Assert
        var snapshot = Assert.Single(page);

        Assert.Equal((user.Id, Guid.Empty), (snapshot.AggregateId, snapshot.Tenant));
        Assert.Equal(("Andres", "Rojas", "1012345678", "CC"), (snapshot.FirstName, snapshot.LastName, snapshot.DocumentNumber, snapshot.DocumentType!.Code));
        Assert.Equal(("Calle 5 # 3-10", "Bogota"), (snapshot.Contact!.Address, snapshot.Contact.City));
    }
}
