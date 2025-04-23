using System;
using FluentValidation.TestHelper;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.AddTenant
{
    public class AddTenantCommandTest
    {
        [Fact]
        public void Validator_Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var validator = new Validator();
            var command = new AddTenantCommand(Guid.Empty, new TenantDto());

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Tenant_Is_Null()
        {
            // Arrange
            var validator = new Validator();
            var command = new AddTenantCommand(Guid.NewGuid(), null!);

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Tenant);
        }

        [Fact]
        public void Validator_Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var validator = new Validator();
            var command = new AddTenantCommand(Guid.NewGuid(), new TenantDto());

            // Act & Assert
            validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
        }
    }
}
