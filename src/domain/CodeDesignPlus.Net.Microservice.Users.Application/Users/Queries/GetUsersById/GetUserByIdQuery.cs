namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetUsersById;

public record GetUserByIdQuery(Guid Id) : IRequest<UsersDto>;

