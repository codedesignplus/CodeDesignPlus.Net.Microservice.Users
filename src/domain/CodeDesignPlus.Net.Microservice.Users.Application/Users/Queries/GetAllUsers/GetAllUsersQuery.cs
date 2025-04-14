namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(Guid Id) : IRequest<UsersDto>;

