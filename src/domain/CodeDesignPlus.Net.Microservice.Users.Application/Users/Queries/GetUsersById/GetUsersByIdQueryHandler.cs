namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetUsersById;

public class GetUsersByIdQueryHandler(IUsersRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetUsersByIdQuery, UsersDto>
{
    public Task<UsersDto> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<UsersDto>(default!);
    }
}
