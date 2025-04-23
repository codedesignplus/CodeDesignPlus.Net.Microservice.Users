namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;

public class GetUsersByIdQueryHandler(IUserRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<GetUsersByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
    {
         ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exists = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exists)
            return await cacheManager.GetAsync<UserDto>(request.Id.ToString());

        var tenant = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(tenant, Errors.UserNotFound);

        var dto = mapper.Map<UserDto>(tenant);

        await cacheManager.SetAsync(request.Id.ToString(), dto);

        return dto;
    }
}
