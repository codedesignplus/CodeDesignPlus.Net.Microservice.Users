
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.gRpc.Services;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Users.gRpc.Test.Services;

public class UserServiceTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _userService = new UserService(_mediatorMock.Object);
    }

    [Fact]
    public async Task AddGroupToUser_ValidRequest_SendsCommand()
    {
        // Arrange
        var requests = new List<gRpc.AddGroupRequest>
        {
            new() { Id = Guid.NewGuid().ToString(), Role = "Admin" }
        };

        var requestStreamMock = new Mock<IAsyncStreamReader<gRpc.AddGroupRequest>>();
        requestStreamMock.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        requestStreamMock.Setup(x => x.Current).Returns(() => requests[0]);

        var context = new Mock<ServerCallContext>();

        // Act
        var result = await _userService.AddGroupToUser(requestStreamMock.Object, context.Object);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<AddRoleCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<Empty>(result);
    }

    [Fact]
    public async Task AddGroupToUser_InvalidId_ThrowsRpcException()
    {
        // Arrange
        var requestStreamMock = new Mock<IAsyncStreamReader<gRpc.AddGroupRequest>>();
        var requests = new List<gRpc.AddGroupRequest>
        {
            new() { Id = "InvalidGuid", Role = "Admin" }
        };
        requestStreamMock.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        requestStreamMock.Setup(x => x.Current).Returns(() => requests[0]);

        var context = new Mock<ServerCallContext>();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _userService.AddGroupToUser(requestStreamMock.Object, context.Object));
    }

    [Fact]
    public async Task AddTenantToUser_ValidRequest_SendsCommand()
    {
        // Arrange
        var requestStreamMock = new Mock<IAsyncStreamReader<gRpc.AddTenantRequest>>();
        var requests = new List<gRpc.AddTenantRequest>
        {
            new() {
                Id = Guid.NewGuid().ToString(),
                Tenant = new gRpc.Tenant { Id = Guid.NewGuid().ToString(), Name = "TenantName" }
            }
        };
        requestStreamMock.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        requestStreamMock.Setup(x => x.Current).Returns(() => requests[0]);

        var context = new Mock<ServerCallContext>();

        // Act
        var result = await _userService.AddTenantToUser(requestStreamMock.Object, context.Object);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<AddTenantCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<Empty>(result);
    }

    [Fact]
    public async Task AddTenantToUser_InvalidId_ThrowsRpcException()
    {
        // Arrange
        var requestStreamMock = new Mock<IAsyncStreamReader<gRpc.AddTenantRequest>>();
        var requests = new List<gRpc.AddTenantRequest>
        {
            new() {
                Id = "InvalidGuid",
                Tenant = new gRpc.Tenant { Id = Guid.NewGuid().ToString(), Name = "TenantName" }
            }
        };
        requestStreamMock.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        requestStreamMock.Setup(x => x.Current).Returns(() => requests[0]);

        var context = new Mock<ServerCallContext>();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _userService.AddTenantToUser(requestStreamMock.Object, context.Object));
    }

    [Fact]
    public async Task AddTenantToUser_InvalidTenantId_ThrowsRpcException()
    {
        // Arrange
        var requestStreamMock = new Mock<IAsyncStreamReader<gRpc.AddTenantRequest>>();
        var requests = new List<gRpc.AddTenantRequest>
        {
            new() {
                Id = Guid.NewGuid().ToString(),
                Tenant = new gRpc.Tenant { Id = "InvalidGuid", Name = "TenantName" }
            }
        };
        requestStreamMock.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        requestStreamMock.Setup(x => x.Current).Returns(() => requests[0]);

        var context = new Mock<ServerCallContext>();

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => _userService.AddTenantToUser(requestStreamMock.Object, context.Object));
    }
}
