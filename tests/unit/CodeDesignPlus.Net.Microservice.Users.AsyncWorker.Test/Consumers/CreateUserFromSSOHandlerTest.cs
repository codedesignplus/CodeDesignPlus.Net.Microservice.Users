using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using MediatR;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Test.Consumers;

public class CreateUserFromSSOHandlerTest
{
    [Fact]
    public async Task HandleAsync_NewUser_CreatesItWithTheDocumentTypeFromTheSignUp()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var repository = new Mock<IUserRepository>();
        var documentType = new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC");
        var data = new UserCreatedFromSSODomainEvent(Guid.NewGuid(), "Jhon", "Castaño", "jhon@fake.com", "3105550131", "Jhon Castaño", "1023456781", documentType, null, null, true, true);
        var handler = new CreateUserFromSSOHandler(mediator.Object, repository.Object, Mock.Of<ILogger<CreateUserFromSSOHandler>>());

        // Act
        await handler.HandleAsync(data, CancellationToken.None);

        // Assert
        mediator.Verify(m => m.Send(It.Is<CreateUserCommand>(c => c.DocumentType == documentType && c.DocumentNumber == "1023456781"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
