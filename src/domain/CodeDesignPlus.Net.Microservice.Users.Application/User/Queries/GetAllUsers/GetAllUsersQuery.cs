using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetAllUsers;

public record GetAllUsersQuery(C.Criteria Criteria) : IRequest<Pagination<UserDto>>;

