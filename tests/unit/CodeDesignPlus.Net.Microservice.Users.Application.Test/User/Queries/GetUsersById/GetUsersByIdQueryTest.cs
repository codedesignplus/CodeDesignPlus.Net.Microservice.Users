using System;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
using FluentValidation.TestHelper;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Queries.GetUsersById;

public class GetUsersByIdQueryTest
{
    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var validator = new Validator();
        var query = new GetUsersByIdQuery(Guid.Empty);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_Id_Is_Valid()
    {
        // Arrange
        var validator = new Validator();
        var query = new GetUsersByIdQuery(Guid.NewGuid());

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
