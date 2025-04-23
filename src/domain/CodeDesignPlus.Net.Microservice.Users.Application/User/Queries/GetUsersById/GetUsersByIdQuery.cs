namespace CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;

public record GetUsersByIdQuery(Guid Id) : IRequest<UserDto>;


public class Validator : AbstractValidator<GetUsersByIdQuery>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}