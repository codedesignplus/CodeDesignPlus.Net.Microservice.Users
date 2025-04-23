using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<GetAllUsersQuery, Pagination<UserDto>>
{
    public async Task<Pagination<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {        
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var tenants = await repository.MatchingAsync<UserAggregate>(request.Criteria, cancellationToken);

        return mapper.Map<Pagination<UserDto>>(tenants);
    }
}
