namespace CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateJob;

[DtoGenerator]
public record UpdateJobCommand(Guid Id, string JobTitle, string CompanyName, string Department, string EmployeeId, string EmployeeType, Instant EmployHireDate, string OfficeLocation) : IRequest;

public class Validator : AbstractValidator<UpdateJobCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
