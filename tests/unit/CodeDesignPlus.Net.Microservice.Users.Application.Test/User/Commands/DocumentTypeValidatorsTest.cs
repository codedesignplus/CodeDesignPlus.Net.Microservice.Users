using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
using FluentValidation.TestHelper;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands;

/// <summary>
/// Los comandos que crean o actualizan un usuario piden el tipo de documento (pendings/663).
/// </summary>
public class DocumentTypeValidatorsTest
{
    [Fact]
    public void CreateUserValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "3105550131", "1023456781", null!, true);

        // Act
        var result = new Application.User.Commands.CreateUser.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void UpdateUserValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "3105550131", "1023456781", null!, true);

        // Act
        var result = new Application.User.Commands.UpdateUser.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void UpdateProfileValidator_WithoutDocumentType_HasError()
    {
        // Arrange
        var contact = ContactInfo.Create("Calle 1", "Bogotá", "Cundinamarca", "Colombia", "110111", "3105550131", ["john.doe@example.com"]);
        var job = JobInfo.Create("Contador", "Kappali", "Finanzas", "1", "Empleado", null, "Bogotá");
        var command = new UpdateProfileCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@example.com", "3105550131", "1023456781", null!, true, contact, job);

        // Act
        var result = new Application.User.Commands.UpdateProfile.Validator().TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }
}
