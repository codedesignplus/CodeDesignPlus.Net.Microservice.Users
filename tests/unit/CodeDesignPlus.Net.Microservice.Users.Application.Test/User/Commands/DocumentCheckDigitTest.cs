using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdateUser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands;

/// <summary>
/// Ningún comando que escribe el documento de un usuario acepta un tipo con dígito de verificación sin guion o con otro
/// dígito, y la configuración del despliegue es la que dice qué tipos y con qué pesos.
/// </summary>
public class DocumentCheckDigitTest
{
    private static readonly IOptions<UserDocumentOptions> Configured = Microsoft.Extensions.Options.Options.Create(new UserDocumentOptions
    {
        CheckDigitDocumentTypes = ["31"],
        CheckDigitWeights = [3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71]
    });

    private static readonly DocumentType TaxId = new(Guid.NewGuid(), "Tax identification number", "31");

    private static readonly DocumentType NationalId = new(Guid.NewGuid(), "National identification", "13");

    private readonly Mock<IUserRepository> repository = new();
    private readonly Mock<IPubSub> pubSub = new();
    private readonly Mock<IUserContext> userContext = new();
    private readonly Mock<ICacheManager> cache = new();

    public DocumentCheckDigitTest()
    {
        userContext.SetupGet(x => x.IdUser).Returns(Guid.NewGuid());
        repository.Setup(x => x.CreateIfAbsentAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task CreateUser_TaxIdWithoutCheckDigit_ThrowsAndDoesNotSave()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john@fake.com", "3105550131", "900765432", TaxId, true);
        var handler = new CreateUserCommandHandler(repository.Object, pubSub.Object, Configured);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(Domain.Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        repository.Verify(x => x.CreateIfAbsentAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_TaxIdThatMayHaveTheDigitAttached_GivesBothReadingsAndDoesNotSave()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john@fake.com", "3105550131", "9018887773", TaxId, true);
        var handler = new CreateUserCommandHandler(repository.Object, pubSub.Object, Configured);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(Domain.Errors.DocumentCheckDigitMayBeAttached.GetCode(), exception.Code);
        Assert.Contains("901888777-3", exception.Error!.Value.GetMessage("es"));
        repository.Verify(x => x.CreateIfAbsentAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_TaxIdWithItsCheckDigit_Saves()
    {
        // Arrange
        var command = new CreateUserCommand(Guid.NewGuid(), "John", "Doe", "JD", "john@fake.com", "3105550131", "901888777-3", TaxId, true);
        var handler = new CreateUserCommandHandler(repository.Object, pubSub.Object, Configured);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repository.Verify(x => x.CreateIfAbsentAsync(It.Is<UserAggregate>(user => user.DocumentNumber == "901888777-3"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_TaxIdWithWrongCheckDigit_ThrowsAndDoesNotSave()
    {
        // Arrange
        var user = this.ExistingUser();
        var command = new UpdateUserCommand(user.Id, "John", "Doe", "JD", "john@fake.com", "3105550131", "901888777-5", TaxId, true);
        var handler = new UpdateUserCommandHandler(repository.Object, userContext.Object, pubSub.Object, cache.Object, Configured);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(Domain.Errors.DocumentCheckDigitDoesNotMatch.GetCode(), exception.Code);
        repository.Verify(x => x.UpdateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfile_TaxIdWithoutCheckDigit_ThrowsAndDoesNotSave()
    {
        // Arrange
        var user = this.ExistingUser();
        var contact = ContactInfo.Create("Street 1", "City", "State", "Country", "110111", "3105550131", ["john@fake.com"]);
        var job = JobInfo.Create("Title", "Company", "Department", "1", "Type", SystemClock.Instance.GetCurrentInstant(), "Office");
        var command = new UpdateProfileCommand(user.Id, "John", "Doe", "JD", "john@fake.com", "3105550131", "900765432", TaxId, true, contact, job);
        var handler = new UpdateProfileCommandHandler(repository.Object, userContext.Object, pubSub.Object, cache.Object, Configured);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(Domain.Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        repository.Verify(x => x.UpdateAsync(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Validate_TypesWithoutWeights_Fails()
    {
        // Arrange
        var options = new UserDocumentOptions { CheckDigitDocumentTypes = ["31"] };

        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, options);

        // Assert
        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_WeightThatIsNotPositive_Fails()
    {
        // Arrange
        var options = new UserDocumentOptions { CheckDigitDocumentTypes = ["31"], CheckDigitWeights = [3, 0, 13] };

        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, options);

        // Assert
        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_EmptyDocumentTypeCode_Fails()
    {
        // Arrange
        var options = new UserDocumentOptions { CheckDigitDocumentTypes = [" "], CheckDigitWeights = [3, 7] };

        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, options);

        // Assert
        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_NoSection_Succeeds()
    {
        // Act
        var result = new UserDocumentOptionsValidator().Validate(null, new UserDocumentOptions());

        // Assert
        Assert.True(result.Succeeded);
    }

    /// <summary>
    /// El valor desplegado. ms-microsoftgraph tiene esta misma prueba con el mismo valor esperado: si una de las dos
    /// configuraciones cambia sin la otra, su prueba falla. Lo que el registro acepta lo tiene que aceptar este micro.
    /// </summary>
    [Theory]
    [InlineData("CodeDesignPlus.Net.Microservice.Users.Rest")]
    [InlineData("CodeDesignPlus.Net.Microservice.Users.AsyncWorker")]
    public void Appsettings_UserDocument_IsTheSharedDeployedValue(string entrypoint)
    {
        // Arrange
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CodeDesignPlus.Net.Microservice.Users.sln")))
        {
            directory = directory.Parent;
        }

        var configuration = new ConfigurationBuilder().AddJsonFile(Path.Combine(directory!.FullName, "src", "entrypoints", entrypoint, "appsettings.json")).Build();

        // Act
        var options = configuration.GetSection(UserDocumentOptions.Section).Get<UserDocumentOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.True(new UserDocumentOptionsValidator().Validate(null, options).Succeeded);
        Assert.Equal(["31"], options.CheckDigitDocumentTypes);
        Assert.Equal([3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71], options.CheckDigitWeights);
    }

    private UserAggregate ExistingUser()
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john@fake.com", "3105550131", "JD", "1023456781", NationalId, true, DocumentCheckDigitRules.None);

        repository.Setup(x => x.FindAsync<UserAggregate>(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        return user;
    }
}
