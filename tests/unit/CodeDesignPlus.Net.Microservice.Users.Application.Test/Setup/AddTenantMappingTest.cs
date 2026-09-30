using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Users.Application.Setup;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.Setup;

/// <summary>
/// The REST endpoint <c>POST /User/{id}/tenant</c> maps its DTO to <see cref="AddTenantCommand"/> with Mapster.
/// </summary>
/// <remarks>
/// The command gained a second constructor (the purchase flow sets <c>ByPurchase</c>) and Mapster could no longer build
/// it: every invitation from the frontend failed with 500 and the user was left without a tenant. Pending 158.
/// </remarks>
public class AddTenantMappingTest
{
    [Fact]
    public void Map_AddTenantDto_BuildsAnInvitationCommand()
    {
        MapsterConfigUsers.Configure();
        var mapper = new Mapper(TypeAdapterConfig.GlobalSettings);
        var tenantId = Guid.Parse("64a1e173-3284-4837-803e-96fab05cdb3c");
        var dto = new AddTenantDto
        {
            UserId = Guid.Parse("646e85eb-7a4d-406f-83ab-cc8cf7d090ba"),
            Tenant = new TenantDto { Id = tenantId, Name = "Conjunto Residencial Cuatro Vientos Demo" },
        };

        var command = mapper.Map<AddTenantCommand>(dto);

        Assert.Equal(dto.UserId, command.UserId);
        Assert.Equal(tenantId, command.Tenant.Id);
        Assert.False(command.ByPurchase);
    }
}
