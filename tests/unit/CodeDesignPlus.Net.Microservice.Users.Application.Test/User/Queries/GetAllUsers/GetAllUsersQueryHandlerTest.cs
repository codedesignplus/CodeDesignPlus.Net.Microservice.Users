using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetAllUsers;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Queries.GetAllUsers;

public class GetAllUsersQueryHandlerTest
{
    private readonly Mock<IUserRepository> repositoryMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly GetAllUsersQueryHandler handler;

    public GetAllUsersQueryHandlerTest()
    {
        repositoryMock = new Mock<IUserRepository>();
        mapperMock = new Mock<IMapper>();
        handler = new GetAllUsersQueryHandler(repositoryMock.Object, mapperMock.Object);
    }

    [Fact]
    public async Task Handle_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        GetAllUsersQuery query = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(query, cancellationToken));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsMappedPagination()
    {
        // Arrange
        var query = new GetAllUsersQuery(null!);
        var cancellationToken = CancellationToken.None;

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true);
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

        var userAggregates = new Pagination<UserAggregate>([aggregate], 1, 10, 0);
        var userDtos = new Pagination<UserDto>([userDto], 1, 10, 0);

        repositoryMock
            .Setup(repo => repo.MatchingAsync<UserAggregate>(query.Criteria, cancellationToken))
            .ReturnsAsync(userAggregates);

        mapperMock
            .Setup(mapper => mapper.Map<Pagination<UserDto>>(userAggregates))
            .Returns(userDtos);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(userDtos, result);
        repositoryMock.Verify(repo => repo.MatchingAsync<UserAggregate>(query.Criteria, cancellationToken), Times.Once);
        mapperMock.Verify(mapper => mapper.Map<Pagination<UserDto>>(userAggregates), Times.Once);
    }

}
