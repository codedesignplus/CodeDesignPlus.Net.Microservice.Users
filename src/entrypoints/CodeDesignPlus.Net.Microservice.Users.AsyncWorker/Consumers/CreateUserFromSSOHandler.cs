using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;

[QueueName<UserAggregate>("CreateUserFromSSO")]
public class CreateUserFromSSOHandler(IMediator mediator, IUserRepository userRepository, ILogger<CreateUserFromSSOHandler> logger) : IEventHandler<UserCreatedFromSSODomainEvent>
{
    public async Task HandleAsync(UserCreatedFromSSODomainEvent data, CancellationToken token)
    {
        var exists = await userRepository.ExistsAsync<UserAggregate>(data.AggregateId, token);

        if (exists)
        {
            logger.LogInformation("User {UserId} already exists in ms-users. Skipping SSO replication.", data.AggregateId);
            return;
        }

        var command = new CreateUserCommand(
            data.AggregateId,
            data.FirstName,
            data.LastName,
            data.DisplayName,
            data.Email,
            data.Phone,
            data.DocumentNumber,
            null,
            data.IsActive
        );

        await mediator.Send(command, token);
    }
}
