namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CompleteOrder;

public class CompleteOrderCommandHandler(IUserRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<CompleteOrderCommand>
{
    public Task Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}