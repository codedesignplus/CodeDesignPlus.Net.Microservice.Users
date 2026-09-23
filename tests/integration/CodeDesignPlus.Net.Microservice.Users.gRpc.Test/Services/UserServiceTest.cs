
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Users.gRpc.Services;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Users.gRpc.Test.Services;

[Collection(ServerCollectionFixture<Program>.Collection)]
public class UserServiceTest : ServerBase<Program>
{
    public UserServiceTest(ServerCollectionFixture<Program> fixture) : base(fixture.Container)
    {
        fixture.Container.InMemoryCollection = (x) =>
        {
            x.Add("Vault:Enable", "false");
            x.Add("Vault:Address", "http://localhost:8200");
            x.Add("Vault:Token", "root");
            x.Add("Solution", "CodeDesignPlus");
            x.Add("AppName", "my-test");
            x.Add("RabbitMQ:UserName", "guest");
            x.Add("RabbitMQ:Password", "guest");
            x.Add("Security:ValidAudiences:0", Guid.NewGuid().ToString());
        };
    }


    [Fact]
    public async Task AddTenant_ReturnEmpty()
    {
        var idTenant = Guid.NewGuid();
        var nameTenant = "Tenant 1";
        var userClient = new Users.UsersClient(Channel);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        var repository = Services.GetRequiredService<IUserRepository>();

        await repository.CreateAsync(aggregate, CancellationToken.None);

        await userClient.AddTenantToUserAsync(new AddTenantRequest
        {
            Id = aggregate.Id.ToString(),
            Tenant = new Tenant
            {
                Id = idTenant.ToString(),
                Name = nameTenant
            }
        });

        var user = await repository.FindAsync<UserAggregate>(aggregate.Id, CancellationToken.None);

        var tenant = user.Tenants.FirstOrDefault(x => x.Id == idTenant);

        Assert.NotNull(tenant);
        Assert.Equal(idTenant, tenant.Id);
        Assert.Equal(nameTenant, tenant.Name);
    }

    
    [Fact]
    public async Task AddGroup_ReturnEmpty()
    {
        var group = Guid.Parse("1a43656c-f457-4695-8bfd-903be4b66097");
        var tenantId = Guid.NewGuid();
        var userClient = new Users.UsersClient(Channel);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", "1234567890", null, true);

        // La copropiedad primero: un rol cuelga de ella, y darselo a quien no pertenece se rechaza.
        aggregate.AddTenant(tenantId, "Malpelo XXI", aggregate.Id);

        var repository = Services.GetRequiredService<IUserRepository>();

        await repository.CreateAsync(aggregate, CancellationToken.None);

        await userClient.AddGroupToUserAsync(new AddGroupRequest
        {
            Id = aggregate.Id.ToString(),
            Role = group.ToString(),
            Tenant = tenantId.ToString()
        });

        var user = await repository.FindAsync<UserAggregate>(aggregate.Id, CancellationToken.None);

        Assert.NotNull(user);

        // El rol vive dentro de su copropiedad, no en la raiz del usuario.
        Assert.Contains(group, user.Tenants.Single(x => x.Id == tenantId).Roles);
        Assert.Empty(user.Roles);
    }
}
