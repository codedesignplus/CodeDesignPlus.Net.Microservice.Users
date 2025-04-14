namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Queries.GetUsersById;

public record GetUsersByIdQuery(Guid Id) : IRequest<UsersDto>;

