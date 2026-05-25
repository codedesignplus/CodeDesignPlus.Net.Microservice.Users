using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

[QueueName<UserAggregate>("CreateUserHandler")]
public class CreateUserHandler(IMediator mediator, IUserRepository userRepository, ILogger<CreateUserHandler> logger) : IEventHandler<UserCreatedDomainEvent>
{
    public async Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<UserAggregate>(data.AggregateId, token);

        if (exists)
        {
            logger.LogInformation("User {UserId} already exists. Skipping.", data.AggregateId);
            return;
        }

        var command = new CreateUserCommand(
            data.AggregateId,
            data.FirstName,
            data.LastName,
            data.DisplayName,
            data.Email,
            data.Phone,
            data.IsActive
        );

        await mediator.Send(command, token);
    }
}
