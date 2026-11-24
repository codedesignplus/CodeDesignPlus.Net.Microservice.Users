using System.Linq;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.Test;

/// <summary>
/// La cuenta que se queda sin nada puede quedar desactivada en vez de borrada, y solo esa desactivación es del sistema.
/// </summary>
public class ReleasedAccountTest
{
    private static readonly Guid Admin = Guid.NewGuid();

    private static UserAggregate User()
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "Laura", "Gómez Ríos", "laura@fake.com", "3105550131", null, "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), true, DocumentCheckDigitRules.None);
        user.GetAndClearEvents();

        return user;
    }

    [Fact]
    public void Release_UserWithNothingLeft_DeactivatesTheAccountAndAnnouncesIt()
    {
        // Arrange
        var user = User();

        // Act
        user.Release(Admin);

        // Assert
        Assert.Equal((false, true, true), (user.IsActive, user.ReleasedAt is not null, user.GetAndClearEvents().OfType<UserDeactivatedDomainEvent>().Any()));
    }

    [Fact]
    public void IsReleasedAndUnused_ReleasedAccount_IsTrue()
    {
        // Arrange
        var user = User();
        user.Release(Admin);

        // Act
        var released = user.IsReleasedAndUnused();

        // Assert
        Assert.True(released);
    }

    [Fact]
    public void IsReleasedAndUnused_DeactivatedByAPerson_IsFalse()
    {
        // Arrange
        var user = User();
        user.Update("Laura", "Gómez Ríos", "laura@fake.com", "3105550131", null, "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), false, Admin, DocumentCheckDigitRules.None);

        // Act
        var released = user.IsReleasedAndUnused();

        // Assert
        Assert.False(released);
    }

    [Fact]
    public void Update_APersonReactivatesAReleasedAccount_ItIsNoLongerTheSystems()
    {
        // Arrange
        var user = User();
        user.Release(Admin);

        // Act
        user.Update("Laura", "Gómez Ríos", "laura@fake.com", "3105550131", null, "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), true, Admin, DocumentCheckDigitRules.None);

        // Assert
        Assert.Null(user.ReleasedAt);
    }

    [Fact]
    public void Update_ReleasedAccountEditedWithoutChangingItsState_StaysReleased()
    {
        // Arrange
        var user = User();
        user.Release(Admin);

        // Act
        user.Update("Laura", "Gómez", "laura@fake.com", "3105550131", null, "1023456781", new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC"), false, Admin, DocumentCheckDigitRules.None);

        // Assert
        Assert.True(user.IsReleasedAndUnused());
    }

    /// <summary>
    /// Los usuarios guardados no traen el campo: se leen como no liberados (regla 31).
    /// </summary>
    [Fact]
    public void Deserialize_UserWithoutReleasedAt_ReadsItAsAbsent()
    {
        // Arrange
        CodeDesignPlus.Net.Mongo.Extensions.MongoSerializerRegistration.RegisterSerializers();

        var user = User();
        user.Release(Admin);

        var document = MongoDB.Bson.BsonExtensionMethods.ToBsonDocument(user);
        document.Remove("ReleasedAt");

        // Act
        var read = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<UserAggregate>(document);

        // Assert
        Assert.Null(read.ReleasedAt);
    }
}
