using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using FluentValidation.TestHelper;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.UpdateProfile;

public class UpdateProfileCommandTest
{
    private readonly Validator validator;

    public UpdateProfileCommandTest()
    {
        validator = new Validator();
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Id_Is_Empty()
    {
        var command = new UpdateProfileCommand(Guid.Empty, "image", "FirstName", "LastName", "DisplayName", "email@example.com", "1234567890", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_FirstName_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "", "LastName", "DisplayName", "email@example.com", "1234567890", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_LastName_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "FirstName", "", "DisplayName", "email@example.com", "1234567890", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Email_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "FirstName", "LastName", "DisplayName", "", "1234567890", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Phone_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "FirstName", "LastName", "DisplayName", "email@example.com", "", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Image_Is_Null_Or_Empty()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "", "FirstName", "LastName", "DisplayName", "email@example.com", "1234567890", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Image);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Contact_Is_Null()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "FirstName", "LastName", "DisplayName", "email@example.com", "1234567890", true, null!, new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Contact);
    }

    [Fact]
    public void Validator_Should_Have_Error_When_Job_Is_Null()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "FirstName", "LastName", "DisplayName", "email@example.com", "1234567890", true, new ContactInfo(), null);
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Job);
    }

    [Fact]
    public void Validator_Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "image", "FirstName", "LastName", "DisplayName", "email@example.com", "1234567890", true, new ContactInfo(), new JobInfo());
        var result = validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
