using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateContact;
using FluentValidation.TestHelper;
namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateContact;

public class UpdateContactCommandTest
{
    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new Validator();
        var command = new UpdateContactCommand(Guid.Empty, "Address", "City", "State", "Country", "PostalCode", "Phone", new string[] { "email@example.com" });

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
        var command = new UpdateContactCommand(Guid.NewGuid(), "Address", "City", "State", "Country", "PostalCode", "Phone", new string[] { "email@example.com" });

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
