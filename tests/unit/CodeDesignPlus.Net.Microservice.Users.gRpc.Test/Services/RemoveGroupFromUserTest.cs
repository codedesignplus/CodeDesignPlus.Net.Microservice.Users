using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
using CodeDesignPlus.Net.Microservice.Users.gRpc;
using Errors = CodeDesignPlus.Net.Microservice.Users.Application.Errors;

namespace CodeDesignPlus.Net.Microservice.Users.gRpc.Test.Services;

/// <summary>
/// Quitar un rol a un usuario que ya no existe no es un error: no tiene nada que perder. Pasa al borrar a un portero
/// cuyo contrato ya se terminó y cuya cuenta ya se borró (pendings/211).
/// </summary>
public class RemoveGroupFromUserTest
{
    private static ServerCallContext Context() => TestServerCallContext.Create(
        "RemoveGroupFromUser", null, DateTime.UtcNow.AddMinutes(1), [], CancellationToken.None, "127.0.0.1", null, null, _ => Task.CompletedTask, () => null, _ => { });

    [Fact]
    public async Task RemoveGroupFromUser_UserNoLongerExists_DoesNothing()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<GetUsersByIdQuery>(), It.IsAny<CancellationToken>()))
            .Returns<GetUsersByIdQuery, CancellationToken>((_, _) =>
            {
                ApplicationGuard.IsNull((object?)null, Errors.UserNotFound);
                return Task.FromResult<UserDto>(null!);
            });
        var service = new UserService(mediator.Object);

        // Act
        var result = await service.RemoveGroupFromUser(new RemoveGroupRequest { Id = Guid.NewGuid().ToString(), Role = Guid.NewGuid().ToString(), Tenant = Guid.NewGuid().ToString() }, Context());

        // Assert
        Assert.NotNull(result);
        mediator.Verify(m => m.Send(It.IsAny<RemoveRoleCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
