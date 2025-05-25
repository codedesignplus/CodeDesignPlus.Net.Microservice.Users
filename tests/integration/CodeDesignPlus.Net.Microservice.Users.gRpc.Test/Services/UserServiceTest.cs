
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
    public async Task AddTenant_ClientStreaming_ReturnEmpty()
    {
        var idTenant = Guid.NewGuid();
        var nameTenant = "Tenant 1";
        var userClient = new Users.UsersClient(Channel);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());

        var repository = Services.GetRequiredService<IUserRepository>();

        await repository.CreateAsync(aggregate, CancellationToken.None);

        using var streamingCall = userClient.AddTenantToUser();

        await streamingCall.RequestStream.WriteAsync(new AddTenantRequest
        {
            Id = aggregate.Id.ToString(),
            Tenant = new Tenant
            {
                Id = idTenant.ToString(),
                Name = nameTenant
            }
        });

        await Task.Delay(2000);

        await streamingCall.RequestStream.CompleteAsync();

        var user = await repository.FindAsync<UserAggregate>(aggregate.Id, CancellationToken.None);

        var tenant = user.Tenants.FirstOrDefault(x => x.Id == idTenant);

        Assert.NotNull(tenant);
        Assert.Equal(idTenant, tenant.Id);
        Assert.Equal(nameTenant, tenant.Name);
    }

    
    [Fact]
    public async Task AddGroup_ClientStreaming_ReturnEmpty()
    {
        var group = "Admin";
        var userClient = new Users.UsersClient(Channel);

        var aggregate = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "1234567890", "JD", true, Guid.NewGuid());

        var repository = Services.GetRequiredService<IUserRepository>();

        await repository.CreateAsync(aggregate, CancellationToken.None);

        using var streamingCall = userClient.AddGroupToUser();

        await streamingCall.RequestStream.WriteAsync(new AddGroupRequest
        {
            Id = aggregate.Id.ToString(),
            Role = group
        });

        await Task.Delay(2000);

        await streamingCall.RequestStream.CompleteAsync();

        var user = await repository.FindAsync<UserAggregate>(aggregate.Id, CancellationToken.None);

        Assert.NotNull(user);
        Assert.Contains(user.Roles, x => x == group);
    }
}
