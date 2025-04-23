using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteUser;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.DeleteUser;

public class DeleteUserCommandTest
{
    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new Validator();
        var command = new DeleteUserCommand(Guid.Empty);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var validator = new Validator();
        var command = new DeleteUserCommand(Guid.NewGuid());

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
