using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Queries.GetUsersById;

public class GetUsersByIdQueryHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<ICacheManager> cacheManagerMock;
    private readonly GetUsersByIdQueryHandler handler;

    public GetUsersByIdQueryHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        mapperMock = new Mock<IMapper>();
        cacheManagerMock = new Mock<ICacheManager>();
        handler = new GetUsersByIdQueryHandler(repositoryMock.Object, mapperMock.Object, cacheManagerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        GetUsersByIdQuery request = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserExistsInCache_ReturnsCachedUser()
    {
        // Arrange
        var request = new GetUsersByIdQuery(Guid.NewGuid());        
        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());
        var userDto = new UserDto
        {
            Id = aggregate.Id,
            FirstName = aggregate.FirstName,
            LastName = aggregate.LastName,
            Email = aggregate.Email,
            Phone = aggregate.Phone,
            Contact = aggregate.Contact,
            DisplayName = aggregate.DisplayName,
            Job = aggregate.Job,
            Tenants = [],
            Roles = aggregate.Roles,
        };
        cacheManagerMock.Setup(x => x.ExistsAsync(request.Id.ToString())).ReturnsAsync(true);
        cacheManagerMock.Setup(x => x.GetAsync<UserDto>(request.Id.ToString())).ReturnsAsync(userDto);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(userDto, result);
        repositoryMock.Verify(x => x.FindAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotInCacheAndNotFoundInRepository_ThrowsUserNotFoundException()
    {
        // Arrange
         var request = new GetUsersByIdQuery(Guid.NewGuid());        
        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());
        var userDto = new UserDto
        {
            Id = aggregate.Id,
            FirstName = aggregate.FirstName,
            LastName = aggregate.LastName,
            Email = aggregate.Email,
            Phone = aggregate.Phone,
            Contact = aggregate.Contact,
            DisplayName = aggregate.DisplayName,
            Job = aggregate.Job,
            Tenants = [],
            Roles = aggregate.Roles,
        };
        cacheManagerMock.Setup(x => x.ExistsAsync(request.Id.ToString())).ReturnsAsync(false);
        repositoryMock.Setup(x => x.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotInCacheButFoundInRepository_ReturnsMappedUserAndCachesIt()
    {
        // Arrange
        var request = new GetUsersByIdQuery(Guid.NewGuid());        
        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());
        var userDto = new UserDto
        {
            Id = aggregate.Id,
            FirstName = aggregate.FirstName,
            LastName = aggregate.LastName,
            Email = aggregate.Email,
            Phone = aggregate.Phone,
            Contact = aggregate.Contact,
            DisplayName = aggregate.DisplayName,
            Job = aggregate.Job,
            Tenants = [],
            Roles = aggregate.Roles,
        };

        cacheManagerMock.Setup(x => x.ExistsAsync(request.Id.ToString())).ReturnsAsync(false);
        repositoryMock.Setup(x => x.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>())).ReturnsAsync(aggregate);
        mapperMock.Setup(x => x.Map<UserDto>(aggregate)).Returns(userDto);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(userDto, result);
        cacheManagerMock.Verify(x => x.SetAsync(request.Id.ToString(), userDto, It.IsAny<TimeSpan?>()), Times.Once);
    }
}
