using System;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateJob;
using FluentValidation.TestHelper;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateJob;

public class UpdateJobCommandTest
{
    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new Validator();
        var command = new UpdateJobCommand(Guid.Empty, "JobTitle", "CompanyName", "Department", "EmployeeId", "EmployeeType", default, "OfficeLocation");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var validator = new Validator();
        var command = new UpdateJobCommand(Guid.NewGuid(), "JobTitle", "CompanyName", "Department", "EmployeeId", "EmployeeType", default, "OfficeLocation");

        // Act & Assert
        var result = validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
