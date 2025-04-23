using System;
using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using NodaTime.Serialization.SystemTextJson;

namespace CodeDesignPlus.Net.Microservice.Users.Rest.Test.Controllers;

public class UserControllerTest : ServerBase<Program>, IClassFixture<Server<Program>>
{
    
    private readonly System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
    }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);


    public UserControllerTest(Server<Program> server) : base(server)
    {
        server.InMemoryCollection = (x) =>
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
    public async Task GetUsers_ReturnOk()
    {
        var User = await this.CreateUserAsync();

        var response = await this.RequestAsync("http://localhost/api/User", null, HttpMethod.Get);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        var users = System.Text.Json.JsonSerializer.Deserialize<Pagination<UserDto>>(json, this.options);

        Assert.NotNull(users);
        Assert.NotEmpty(users.Data);
        Assert.Contains(users.Data, x => x.Id == User.Id);
    }

    [Fact]
    public async Task GetUserById_ReturnOk()
    {
        var userCreated = await this.CreateUserAsync();

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}", null, HttpMethod.Get);

        var json = await response.Content.ReadAsStringAsync();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        var user = System.Text.Json.JsonSerializer.Deserialize<UserDto>(json, this.options);

        Assert.NotNull(user);
        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, user.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);
    }

    [Fact]
    public async Task CreateUser_ReturnNoContent()
    {
        var data = new CreateUserDto()
        {
            Id = Guid.NewGuid(),
            FirstName = "Joe",
            LastName = "Doe",
            DisplayName = "Joe Doe",
            Phone = "123456789",
            Email = "joe@fake.com"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync("http://localhost/api/User", content, HttpMethod.Post);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(data.Id, user.Id);
        Assert.Equal(data.FirstName, user.FirstName);
        Assert.Equal(data.LastName, user.LastName);
        Assert.Equal(data.Email, user.Email);
        Assert.Equal(data.Phone, user.Phone);
        Assert.Equal(data.DisplayName, user.DisplayName);
    }

    [Fact]
    public async Task UpdateUser_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();

        var data = new UpdateUserDto()
        {
            Id = userCreated.Id,
            DisplayName = "Bart Simpson",
            FirstName = "Bart",
            LastName = "Simpson",
            Phone = "123456700",
            Email = "bart@fake.com",
            IsActive = true,
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}", content, HttpMethod.Put);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(data.Id, user.Id);
        Assert.Equal(data.FirstName, user.FirstName);
        Assert.Equal(data.LastName, user.LastName);
        Assert.Equal(data.Email, user.Email);
        Assert.Equal(data.Phone, user.Phone);
        Assert.Equal(data.DisplayName, user.DisplayName);
    }

    [Fact]
    public async Task AddTenant_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();

        var data = new AddTenantDto()
        {
            Id = userCreated.Id,
            Tenant = new TenantDto()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 1",
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/tenant", content, HttpMethod.Post);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, userCreated.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);

        Assert.Contains(user.Tenants, x => x.Id == data.Tenant.Id && x.Name == data.Tenant.Name);
    }


    [Fact]
    public async Task RemoveTenant_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();

        var data = new AddTenantDto()
        {
            Id = userCreated.Id,
            Tenant = new TenantDto()
            {
                Id = Guid.NewGuid(),
                Name = "Tenant 1",
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var responseAddTenant = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/tenant", content, HttpMethod.Post);

        Assert.NotNull(responseAddTenant);
        Assert.Equal(HttpStatusCode.NoContent, responseAddTenant.StatusCode);

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/tenant/{data.Tenant.Id}", content, HttpMethod.Delete);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, user.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);

        Assert.Empty(user.Tenants);
    }


    [Fact]
    public async Task AddRole_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();

        var data = new AddRoleDto
        {
            Id = userCreated.Id,
            Role = "Admin"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/role", content, HttpMethod.Post);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, user.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);

        Assert.Contains(user.Roles, x => x == data.Role);
    }


    [Fact]
    public async Task RemoveRole_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();


        var data = new AddRoleDto
        {
            Id = userCreated.Id,
            Role = "Admin"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var responseAddRole = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/role", content, HttpMethod.Post);

        Assert.NotNull(responseAddRole);
        Assert.Equal(HttpStatusCode.NoContent, responseAddRole.StatusCode);

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/role/{data.Role}", content, HttpMethod.Delete);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, user.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);

        Assert.Empty(user.Roles);
    }


    [Fact]
    public async Task UpdateContact_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();

        var data = new UpdateContactDto
        {
            Id = userCreated.Id,
            Address = "123 Main St",
            City = "Sample City",
            State = "Sample State",
            Country = "Sample Country",
            PostalCode = "12345",
            Phone = "123-456-7890",
            Email = ["fake@fake.com"]
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/contact", content, HttpMethod.Patch);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, user.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);

        Assert.Equal(data.Id, user.Id);
        Assert.Equal(data.Address, user.Contact.Address);
        Assert.Equal(data.City, user.Contact.City);
        Assert.Equal(data.State, user.Contact.State);
        Assert.Equal(data.Country, user.Contact.Country);
        Assert.Equal(data.PostalCode, user.Contact.PostalCode);
        Assert.Equal(data.Phone, user.Contact.Phone);
        Assert.Equal(data.Email, user.Contact.Email);
    }


    [Fact]
    public async Task UpdateJob_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();

        var data = new UpdateJobDto
        {
            Id = userCreated.Id,
            JobTitle = "Software Engineer",
            CompanyName = "TechCorp",
            Department = "IT",
            EmployeeId = "12345",
            EmployeeType = "Full-Time",
            EmployHireDate = SystemClock.Instance.GetCurrentInstant(),
            OfficeLocation = "HQ"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/job", content, HttpMethod.Patch);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(userCreated.Id, user.Id);
        Assert.Equal(userCreated.FirstName, userCreated.FirstName);
        Assert.Equal(userCreated.LastName, user.LastName);
        Assert.Equal(userCreated.Email, user.Email);
        Assert.Equal(userCreated.Phone, user.Phone);
        Assert.Equal(userCreated.DisplayName, user.DisplayName);

        Assert.Equal(data.Id, user.Id);
        Assert.Equal(data.JobTitle, user.Job.JobTitle);
        Assert.Equal(data.CompanyName, user.Job.CompanyName);
        Assert.Equal(data.Department, user.Job.Department);
        Assert.Equal(data.EmployeeId, user.Job.EmployeeId);
        Assert.Equal(data.EmployeeType, user.Job.EmployeeType);
        Assert.Equal(data.EmployHireDate.ToString(), user.Job.EmployHireDate.ToString());
        Assert.Equal(data.OfficeLocation, user.Job.OfficeLocation);
    }


    [Fact]
    public async Task UpdateProfile_ReturnNoContent()
    {
        var userCreated = await this.CreateUserAsync();


        var dataContact = Domain.ValueObjects.ContactInfo.Create(
            "123 Main St",
            "Sample City",
            "Sample State",
            "Sample Country",
            "12345",
            "123-456-7890",
            ["fake@fake.com"]);

        var dataJob = Domain.ValueObjects.JobInfo.Create(
            "Software Engineer",
            "TechCorp",
            "IT",
            "12345",
            "Full-Time",
            SystemClock.Instance.GetCurrentInstant(),
            "HQ"
        );

        var data = new UpdateProfileDto()
        {
            Id = userCreated.Id,
            DisplayName = "Bart Simpson",
            FirstName = "Bart",
            LastName = "Simpson",
            Phone = "123456700",
            Email = "bart@fake.com",
            IsActive = true,
            Contact = dataContact,
            Job = dataJob,
            Image = "https://example.com/image.jpg",
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await this.RequestAsync($"http://localhost/api/User/{userCreated.Id}/profile", content, HttpMethod.Put);

        var user = await this.GetRecordAsync(data.Id);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(data.Id, user.Id);
        Assert.Equal(data.FirstName, user.FirstName);
        Assert.Equal(data.LastName, user.LastName);
        Assert.Equal(data.Email, user.Email);
        Assert.Equal(data.Phone, user.Phone);
        Assert.Equal(data.DisplayName, user.DisplayName);
        Assert.Equal(data.Image, user.Image);
        
        Assert.Equal(dataContact.Address, user.Contact.Address);
        Assert.Equal(dataContact.City, user.Contact.City);
        Assert.Equal(dataContact.State, user.Contact.State);
        Assert.Equal(dataContact.Country, user.Contact.Country);
        Assert.Equal(dataContact.PostalCode, user.Contact.PostalCode);
        Assert.Equal(dataContact.Phone, user.Contact.Phone);
        Assert.Equal(dataContact.Email, user.Contact.Email);

        Assert.Equal(dataJob.JobTitle, user.Job.JobTitle);
        Assert.Equal(dataJob.CompanyName, user.Job.CompanyName);
        Assert.Equal(dataJob.Department, user.Job.Department);
        Assert.Equal(dataJob.EmployeeId, user.Job.EmployeeId);
        Assert.Equal(dataJob.EmployeeType, user.Job.EmployeeType);
        Assert.Equal(dataJob.EmployHireDate.ToString(), user.Job.EmployHireDate.ToString());
        Assert.Equal(dataJob.OfficeLocation, user.Job.OfficeLocation);
    }

    [Fact]
    public async Task DeleteUser_ReturnNoContent()
    {
        var UserCreated = await this.CreateUserAsync();

        var response = await this.RequestAsync($"http://localhost/api/User/{UserCreated.Id}", null, HttpMethod.Delete);

        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<CreateUserDto> CreateUserAsync()
    {
        var data = new CreateUserDto()
        {
            Id = Guid.NewGuid(),
            FirstName = "Joe",
            LastName = "Doe",
            DisplayName = "Joe Doe",
            Phone = "123456789",
            Email = "joe@fake.com"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, this.options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await this.RequestAsync("http://localhost/api/User", content, HttpMethod.Post);

        return data;
    }

    private async Task<UserDto> GetRecordAsync(Guid id)
    {
        var response = await this.RequestAsync($"http://localhost/api/User/{id}", null, HttpMethod.Get);

        var json = await response.Content.ReadAsStringAsync();

        return System.Text.Json.JsonSerializer.Deserialize<UserDto>(json, this.options)!;
    }

    private async Task<HttpResponseMessage> RequestAsync(string uri, HttpContent? content, HttpMethod method)
    {
        var httpRequestMessage = new HttpRequestMessage()
        {
            RequestUri = new Uri(uri),
            Content = content,
            Method = method
        };
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("TestAuth");

        var response = await Client.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            throw new Exception(data);
        }

        return response;
    }

}
