using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateJob;

public class UpdateJobCommandHandlerTest
{
    [Fact]
    public async Task Handle_RequestIsNull_ThrowsInvalidRequestException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var handler = new UpdateJobCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(null!, CancellationToken.None));

        Assert.Equal(Errors.InvalidRequest.GetMessage(), exception.Message);
        Assert.Equal(Errors.InvalidRequest.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var handler = new UpdateJobCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        var request = new UpdateJobCommand(
            Id: Guid.NewGuid(),
            JobTitle: "Software Engineer",
            CompanyName: "TechCorp",
            Department: "IT",
            EmployeeId: "12345",
            EmployeeType: "Full-Time",
            EmployHireDate: SystemClock.Instance.GetCurrentInstant(),
            OfficeLocation: "HQ"
        );

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((UserAggregate)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(Errors.UserNotFound.GetMessage(), exception.Message);
        Assert.Equal(Errors.UserNotFound.GetCode(), exception.Code);
        Assert.Equal(Layer.Application, exception.Layer);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesUserAndPublishesEvents()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var pubSubMock = new Mock<IPubSub>();
        var handler = new UpdateJobCommandHandler(repositoryMock.Object, userContextMock.Object, pubSubMock.Object);

        var request = new UpdateJobCommand(
           Id: Guid.NewGuid(),
           JobTitle: "Software Engineer",
           CompanyName: "TechCorp",
           Department: "IT",
           EmployeeId: "12345",
           EmployeeType: "Full-Time",
           EmployHireDate: SystemClock.Instance.GetCurrentInstant(),
           OfficeLocation: "HQ"
       );

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());

        repositoryMock.Setup(r => r.FindAsync<UserAggregate>(request.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(aggregate);

        userContextMock.SetupGet(u => u.IdUser).Returns(Guid.NewGuid());


        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.UpdateAsync(aggregate, It.IsAny<CancellationToken>()), Times.Once);
        pubSubMock.Verify(p => p.PublishAsync(It.IsAny<List<JobInfoUpdatedDomainEvent>>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);
    }
}
