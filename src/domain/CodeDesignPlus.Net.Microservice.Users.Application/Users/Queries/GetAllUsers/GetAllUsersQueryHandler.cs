using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUsersRepository repository, IMapper mapper) : IRequestHandler<GetAllUsersQuery, Pagination<UsersDto>>
{
    public async Task<Pagination<UsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var users = await repository.MatchingAsync<UsersAggregate>(request.Criteria, cancellationToken);

        return mapper.Map<Pagination<UsersDto>>(users);
    }
}
