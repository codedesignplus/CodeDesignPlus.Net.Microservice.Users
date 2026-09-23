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
        var command = new AddRoleCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Role_Is_Empty()
    {
        // Arrange
        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_TenantId_Is_Empty()
    {
        // Arrange: un rol sin copropiedad no existe.
        var command = new AddRoleCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TenantId);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new AddRoleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
