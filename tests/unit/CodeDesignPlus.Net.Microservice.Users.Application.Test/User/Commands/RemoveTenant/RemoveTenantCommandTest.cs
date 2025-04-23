using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveTenant;
using FluentValidation.TestHelper;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.RemoveTenant;

public class RemoveTenantCommandTest
{
    private readonly Validator validator;

    public RemoveTenantCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new RemoveTenantCommand(Guid.Empty, Guid.NewGuid());

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_IdTenant_Is_Empty()
    {
        // Arrange
        var command = new RemoveTenantCommand(Guid.NewGuid(), Guid.Empty);

        // Act & Assert
        validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.IdTenant);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Ids_Are_Valid()
    {
        // Arrange
        var command = new RemoveTenantCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }
}
