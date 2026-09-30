using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateContact;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdatePicture;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Setup;

public static class MapsterConfigUsers
{
    public static void Configure()
    {

        TypeAdapterConfig<TenantEntity, TenantDto>.NewConfig();
        TypeAdapterConfig<UserAggregate, UserDto>.NewConfig();

        TypeAdapterConfig<CreateUserDto, CreateUserCommand>.NewConfig();
        TypeAdapterConfig<UpdateUserDto, UpdateUserCommand>.NewConfig();
        TypeAdapterConfig<UpdatePictureDto, UpdatePictureCommand>.NewConfig();

        TypeAdapterConfig<AddRoleDto, AddRoleCommand>.NewConfig();

        // AddTenantCommand has two constructors (the purchase flow sets ByPurchase), so Mapster cannot pick one on its
        // own: without MapWith every POST /User/{id}/tenant failed with 500 and no invitation reached a tenant.
        TypeAdapterConfig<AddTenantDto, AddTenantCommand>
            .NewConfig()
            .MapWith(src => new AddTenantCommand(src.UserId, src.Tenant));

        TypeAdapterConfig<UpdateContactDto, UpdateContactCommand>.NewConfig();
        TypeAdapterConfig<UpdateJobDto, UpdateJobCommand>.NewConfig();

        TypeAdapterConfig<UpdateProfileDto, UpdateProfileCommand>
            .NewConfig()
            .MapWith(src => new UpdateProfileCommand(src.Id, src.FirstName, src.LastName, src.DisplayName, src.Email, src.Phone, src.DocumentNumber, src.DocumentType, src.IsActive, src.Contact, src.Job));
    }
}
