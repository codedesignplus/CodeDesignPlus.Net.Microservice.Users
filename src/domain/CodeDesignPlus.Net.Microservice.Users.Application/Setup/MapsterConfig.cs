using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateContact;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Setup;

public static class MapsterConfigUsers
{
    public static void Configure() { 

        TypeAdapterConfig<TenantEntity, TenantDto>.NewConfig();
        TypeAdapterConfig<UserAggregate, UserDto>.NewConfig();

        TypeAdapterConfig<CreateUserDto, CreateUserCommand>.NewConfig();
        TypeAdapterConfig<UpdateUserDto, UpdateUserCommand>.NewConfig();

        TypeAdapterConfig<AddRoleDto, AddRoleCommand>.NewConfig();

        TypeAdapterConfig<AddTenantDto, AddTenantCommand>.NewConfig();

        TypeAdapterConfig<UpdateContactDto,UpdateContactCommand>.NewConfig();
        TypeAdapterConfig<UpdateJobDto,UpdateJobCommand>.NewConfig();

        TypeAdapterConfig<UpdateProfileDto,UpdateProfileCommand>
            .NewConfig()
            .MapWith(src => new UpdateProfileCommand(src.Id, src.Image, src.FirstName, src.LastName, src.DisplayName, src.Email, src.Phone, src.IsActive, src.Contact, src.Job));
    }
}
