namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetUsersById;

public class GetUserByIdQueryHandler(IUsersRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<GetUserByIdQuery, UsersDto>
{
    public async Task<UsersDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exists = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exists)
            return await cacheManager.GetAsync<UsersDto>(request.Id.ToString());

        var user = await repository.FindAsync<UsersAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        await cacheManager.SetAsync(request.Id.ToString(), mapper.Map<UsersDto>(user));

        return mapper.Map<UsersDto>(user);
    }
}
