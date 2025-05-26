using System;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateContact;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetAllUsers;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using CodeDesignPlus.Net.Microservice.Users.Rest.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Rest.Test.Controllers;

public class UserControllerTest
{
    private readonly Mock<IMediator> mediatorMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly UserController controller;

    private readonly UserAggregate aggregate;
    private readonly UserDto userDto;

    public UserControllerTest()
    {
        mediatorMock = new Mock<IMediator>();
        mapperMock = new Mock<IMapper>();
        controller = new UserController(mediatorMock.Object, mapperMock.Object);

        this.aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true);
        this.userDto = new()
        {
            Id = aggregate.Id,
            FirstName = aggregate.FirstName,
            LastName = aggregate.LastName,
            Email = aggregate.Email,
            Phone = aggregate.Phone,
            Contact = aggregate.Contact,
            DisplayName = aggregate.DisplayName,
            Job = aggregate.Job,
            Tenants = [],
            Roles = aggregate.Roles,
        };
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult()
    {
        // Arrange
        var criteria = new C.Criteria();
        var cancellationToken = CancellationToken.None;
        var expectedResult = new Pagination<UserDto>([this.userDto], 0, 0, 0);
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.GetUsers(criteria, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);

        mediatorMock.Verify(m => m.Send(It.IsAny<GetAllUsersQuery>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var expectedResult = this.userDto;
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUsersByIdQuery>(), cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.GetUserById(userId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);

        mediatorMock.Verify(m => m.Send(It.IsAny<GetUsersByIdQuery>(), cancellationToken), Times.Once);
    }

    // [Fact]
    // public async Task CreateUser_ReturnsNoContent()
    // {
    //     // Arrange
    //     var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john.doe@fake.com", "1234567890");
    //     var createUserDto = new CodeDesignPlus.Microservice.Api.Dtos.CreateUserDto()
    //     {
    //         Id = command.Id,
    //         FirstName = command.FirstName,
    //         LastName = command.LastName,
    //         Email = command.Email,
    //         Phone = command.Phone,
    //         DisplayName = command.DisplayName
    //     };

    //     var cancellationToken = CancellationToken.None;

    //     mapperMock
    //         .Setup(m => m.Map<CreateUserCommand>(createUserDto))
    //         .Returns(command);

    //     // Act
    //     var result = await controller.CreateUser(createUserDto, cancellationToken);

    //     // Assert
    //     Assert.IsType<NoContentResult>(result);

    //     mediatorMock.Verify(m => m.Send(command, cancellationToken), Times.Once);
    // }

    [Fact]
    public async Task UpdateUser_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(Guid.NewGuid(), "John U", "Doe U", "JDU", "john@fake.com", "1234567890", true);

        var updateUserDto = new CodeDesignPlus.Microservice.Api.Dtos.UpdateUserDto()
        {
            Id = command.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Phone = command.Phone,
            DisplayName = command.DisplayName,
            IsActive = command.IsActive
        };
        var cancellationToken = CancellationToken.None;

        mapperMock
            .Setup(m => m.Map<UpdateUserCommand>(updateUserDto))
            .Returns(command);

        // Act
        var result = await controller.UpdateUser(userId, updateUserDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(command, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteUser(userId, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(It.IsAny<DeleteUserCommand>(), cancellationToken), Times.Once);
    }


    [Fact]
    public async Task AddTenant_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var addTenantDto = new CodeDesignPlus.Microservice.Api.Dtos.AddTenantDto
        {
            Id = Guid.NewGuid(),
            Tenant = new()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant Name",
            }
        };
        var addTenantCommand = new AddTenantCommand(

            addTenantDto.Id,
            new()
            {
                Id = addTenantDto.Tenant.Id,
                Name = addTenantDto.Tenant.Name,
            }
        );

        var cancellationToken = CancellationToken.None;

        mapperMock
            .Setup(m => m.Map<AddTenantCommand>(addTenantDto))
            .Returns(addTenantCommand);

        // Act
        var result = await controller.AddTenant(userId, addTenantDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(addTenantCommand, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RemoveTenant_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        mediatorMock
            .Setup(m => m.Send(It.IsAny<RemoveTenantCommand>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.RemoveTenant(userId, tenantId, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(It.IsAny<RemoveTenantCommand>(), cancellationToken), Times.Once);
    }


    [Fact]
    public async Task AddRole_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var addRoleDto = new CodeDesignPlus.Microservice.Api.Dtos.AddRoleDto
        {
            Id = userId,
            Role = "Admin"
        };
        var addRoleCommand = new AddRoleCommand(userId, addRoleDto.Role);
        var cancellationToken = CancellationToken.None;

        mapperMock
            .Setup(m => m.Map<AddRoleCommand>(addRoleDto))
            .Returns(addRoleCommand);

        // Act
        var result = await controller.AddRole(userId, addRoleDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(addRoleCommand, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RemoveRole_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = "Admin";
        var cancellationToken = CancellationToken.None;

        mediatorMock
            .Setup(m => m.Send(It.IsAny<RemoveRoleCommand>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await controller.RemoveRole(userId, role, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(It.IsAny<RemoveRoleCommand>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateContact_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateContactDto = new CodeDesignPlus.Microservice.Api.Dtos.UpdateContactDto
        {
            Id = Guid.NewGuid(),
            Address = "123 Main St",
            City = "Sample City",
            State = "Sample State",
            Country = "Sample Country",
            PostalCode = "12345",
            Phone = "123-456-7890",
            Email = ["fake@fake.com"]
        };
        var updateContactCommand = new UpdateContactCommand(updateContactDto.Id, updateContactDto.Address, updateContactDto.City, updateContactDto.State, updateContactDto.Country, updateContactDto.PostalCode, updateContactDto.Phone, updateContactDto.Email);
        var cancellationToken = CancellationToken.None;

        mapperMock
            .Setup(m => m.Map<UpdateContactCommand>(updateContactDto))
            .Returns(updateContactCommand);

        // Act
        var result = await controller.UpdateContact(userId, updateContactDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(updateContactCommand, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateJob_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateJobDto = new CodeDesignPlus.Microservice.Api.Dtos.UpdateJobDto
        {
            Id = Guid.NewGuid(),
            JobTitle = "Software Engineer",
            CompanyName = "TechCorp",
            Department = "IT",
            EmployeeId = "12345",
            EmployeeType = "Full-Time",
            EmployHireDate = SystemClock.Instance.GetCurrentInstant(),
            OfficeLocation = "HQ"
        };

        var updateJobCommand = new UpdateJobCommand(
            updateJobDto.Id,
            updateJobDto.JobTitle,
            updateJobDto.CompanyName,
            updateJobDto.Department,
            updateJobDto.EmployeeId,
            updateJobDto.EmployeeType,
            updateJobDto.EmployHireDate,
            updateJobDto.OfficeLocation
        );
        var cancellationToken = CancellationToken.None;

        mapperMock
            .Setup(m => m.Map<UpdateJobCommand>(updateJobDto))
            .Returns(updateJobCommand);

        // Act
        var result = await controller.UpdateJob(userId, updateJobDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(updateJobCommand, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contactInfo = ContactInfo.Create("Cll 3", "City", "State", "Country", "12345", "+57 3107565142", ["joe@fake.com"]);
        var jobInfo = JobInfo.Create("Developer", "Microsoft", "IT", "1234567890", "Custom", SystemClock.Instance.GetCurrentInstant(), "Office Location");

        var updateProfileDto = new CodeDesignPlus.Microservice.Api.Dtos.UpdateProfileDto
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DisplayName = "JD",
            Email = "joe@fake.com",
            Phone = "1234567890",
            IsActive = true,
            Contact = contactInfo,
            Job = jobInfo
        };
        var updateProfileCommand = new UpdateProfileCommand(
            updateProfileDto.Id,
            updateProfileDto.FirstName,
            updateProfileDto.LastName,
            updateProfileDto.DisplayName,
            updateProfileDto.Email,
            updateProfileDto.Phone,
            updateProfileDto.IsActive,
            contactInfo,
            jobInfo
        );
        var cancellationToken = CancellationToken.None;

        mapperMock
            .Setup(m => m.Map<UpdateProfileCommand>(updateProfileDto))
            .Returns(updateProfileCommand);

        // Act
        var result = await controller.UpdateProfile(userId, updateProfileDto, cancellationToken);

        // Assert
        Assert.IsType<NoContentResult>(result);

        mediatorMock.Verify(m => m.Send(updateProfileCommand, cancellationToken), Times.Once);
    }

}
