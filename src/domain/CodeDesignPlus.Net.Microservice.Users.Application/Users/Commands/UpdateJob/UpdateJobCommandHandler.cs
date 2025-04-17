namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateJob;

public class UpdateJobCommandHandler(IUsersRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<UpdateJobCommand>
{
    public async Task Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.UpdateJobInfo(request.JobTitle, request.CompanyName, request.Department, request.EmployeeId, request.EmployeeType, request.EmployHireDate, request.OfficeLocation, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}