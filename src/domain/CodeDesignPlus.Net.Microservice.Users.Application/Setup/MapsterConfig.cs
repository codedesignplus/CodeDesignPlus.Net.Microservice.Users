using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.Users.Commands.UpdateUser;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Setup;

public static class MapsterConfigUsers
{
    public static void Configure() { 

        TypeAdapterConfig<CreateUserDto, CreateUserCommand>.NewConfig();
        TypeAdapterConfig<UpdateUserDto, UpdateUserCommand>.NewConfig();
        TypeAdapterConfig<UsersAggregate, UsersDto>.NewConfig();
    }
}
