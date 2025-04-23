using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.AddRole;
public class AddRoleCommandTest
{
    private readonly Validator _validator;

    public AddRoleCommandTest()
    {
        _validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new AddRoleCommand(Guid.Empty, "Admin");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Role_Is_Empty()
    {
        // Arrange
        var command = new AddRoleCommand(Guid.NewGuid(), string.Empty);

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new AddRoleCommand(Guid.NewGuid(), "Admin");

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
