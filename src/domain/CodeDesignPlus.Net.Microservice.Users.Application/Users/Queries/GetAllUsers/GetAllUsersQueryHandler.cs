namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUsersRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetAllUsersQuery, UsersDto>
{
    public Task<UsersDto> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<UsersDto>(default!);
    }
}
