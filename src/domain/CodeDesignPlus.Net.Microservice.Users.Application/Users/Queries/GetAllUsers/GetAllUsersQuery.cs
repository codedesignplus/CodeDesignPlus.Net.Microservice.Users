using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(C.Criteria Criteria) : IRequest<Pagination<UsersDto>>;

