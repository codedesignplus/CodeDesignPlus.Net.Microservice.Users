using System;
using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.RemoveRole;

public class RemoveRoleCommandTest
{
    private readonly Validator validator;

    public RemoveRoleCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new RemoveRoleCommand(Guid.Empty, "Admin");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Role_Is_Empty()
    {
        // Arrange
        var command = new RemoveRoleCommand(Guid.NewGuid(), string.Empty);

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new RemoveRoleCommand(Guid.NewGuid(), "Admin");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
