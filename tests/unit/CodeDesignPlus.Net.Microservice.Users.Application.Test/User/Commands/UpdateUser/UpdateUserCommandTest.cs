using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateUser;

public class UpdateUserCommandTest
{
    private readonly Validator validator;

    public UpdateUserCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.Empty, "John", "Doe", "JD", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_FirstName_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "", "Doe", "JD", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_LastName_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "", "JD", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Phone_Is_Empty()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "", true);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "1234567890", true);
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
