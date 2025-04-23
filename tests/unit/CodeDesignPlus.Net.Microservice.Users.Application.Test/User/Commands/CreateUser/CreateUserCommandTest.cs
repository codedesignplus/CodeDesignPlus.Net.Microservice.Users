using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.CreateUser;

public class CreateUserCommandTest
{
    private readonly Validator _validator;

    public CreateUserCommandTest()
    {
        _validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new CreateUserCommand(Guid.Empty, "John", "Doe", "JD", "john.doe@example.com", "1234567890");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new CreateUserCommand(Guid.NewGuid(), "", "Doe", "JD", "john.doe@example.com", "1234567890");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "", "JD", "john.doe@example.com", "1234567890");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "", "1234567890");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Phone_Is_Empty()
    {
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "1234567890");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
