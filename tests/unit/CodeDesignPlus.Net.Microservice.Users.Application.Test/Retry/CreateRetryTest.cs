using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.Retry;

/// <summary>
/// Cada formulario manda el mismo id en cada envío (pendings/589, regla 66): un alta con un id que ya está guardado es el
/// reintento de un envío que funcionó, y termina bien sin guardar ni anunciar nada dos veces. Antes respondía «ya existe».
/// </summary>
public class CreateRetryTest
{
    private readonly Mock<IPubSub> pubsub = new();

    [Fact]
    public async Task CreateUser_TheSameIdAgain_DoesNotStoreNorAnnounce()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.ExistsAsync<UserAggregate>(RetryHarness.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = RetryHarness.Build<CreateUserCommandHandler>(repository.Object, RetryHarness.User(), pubsub.Object);

        // Act
        await handler.Handle(RetryHarness.WithId<CreateUserCommand>(), CancellationToken.None);

        // Assert
        repository.Verify(x => x.CreateIfAbsentAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
        NothingAnnounced();
    }

    private void NothingAnnounced()
    {
        pubsub.Verify(x => x.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
        pubsub.Verify(x => x.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
