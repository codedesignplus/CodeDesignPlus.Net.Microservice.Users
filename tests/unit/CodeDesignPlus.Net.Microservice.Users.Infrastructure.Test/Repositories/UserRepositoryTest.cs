using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Infrastructure.Test.Repositories;

public class UserRepositoryTest
{
    private readonly Mock<IServiceProvider> serviceProviderMock;
    private readonly Mock<IOptions<MongoOptions>> mongoOptionsMock;
    private readonly Mock<ILogger<UserRepository>> loggerMock;

    public UserRepositoryTest()
    {
        serviceProviderMock = new Mock<IServiceProvider>();
        mongoOptionsMock = new Mock<IOptions<MongoOptions>>();
        loggerMock = new Mock<ILogger<UserRepository>>();
    }

    [Fact]
    public void Constructor_ValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mongoOptions = new MongoOptions();
        mongoOptionsMock.Setup(m => m.Value).Returns(mongoOptions);

        // Act
        var userRepository = new UserRepository(
            serviceProviderMock.Object,
            mongoOptionsMock.Object,
            loggerMock.Object
        );

        // Assert
        Assert.NotNull(userRepository);
    }
}
