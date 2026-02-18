using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

[QueueName<UserAggregate>("CreateUserHandler")]
public class CreateUserHandler(IMediator mediator) : IEventHandler<UserCreatedDomainEvent>
{
    public Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        var command = new CreateUserCommand(
            data.AggregateId,
            data.FirstName,
            data.LastName,
            data.DisplayName,
            data.Email,
            data.Phone,
            data.IsActive
        );

        return mediator.Send(command, token);
    }
}
