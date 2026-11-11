using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.Test;

/// <summary>
/// Todo usuario tiene tipo de documento (pendings/663): la factura electronica lleva el tipo y el numero del
/// adquiriente, y la DIAN no admite uno por defecto.
/// </summary>
public class DocumentTypeRequiredTest
{
    private static readonly DocumentType Cedula = new(Guid.NewGuid(), "Cédula de Ciudadanía", "CC");

    public static TheoryData<DocumentType?> MissingTypes => new()
    {
        null,
        new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", ""),
        new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "   "),
    };

    [Theory]
    [MemberData(nameof(MissingTypes))]
    public void Create_WithoutDocumentType_ThrowsDocumentTypeRequired(DocumentType? documentType)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", documentType!, true));

        // Assert
        Assert.Equal(Errors.DocumentTypeRequired.GetCode(), exception.Code);
    }

    [Theory]
    [MemberData(nameof(MissingTypes))]
    public void Update_WithoutDocumentType_ThrowsDocumentTypeRequired(DocumentType? documentType)
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", Cedula, true);

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => user.Update("John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", documentType!, true, Guid.NewGuid()));

        // Assert
        Assert.Equal(Errors.DocumentTypeRequired.GetCode(), exception.Code);
    }

    [Theory]
    [MemberData(nameof(MissingTypes))]
    public void UpdateProfile_WithoutDocumentType_ThrowsDocumentTypeRequired(DocumentType? documentType)
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", Cedula, true);
        var contact = ContactInfo.Create("Calle 1", "Bogotá", "Cundinamarca", "Colombia", "110111", "3105550131", ["john.doe@example.com"]);
        var job = JobInfo.Create("Contador", "Kappali", "Finanzas", "1", "Empleado", SystemClock.Instance.GetCurrentInstant(), "Bogotá");

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => user.UpdateProfile("John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", documentType!, true, contact, job, Guid.NewGuid()));

        // Assert
        Assert.Equal(Errors.DocumentTypeRequired.GetCode(), exception.Code);
    }

    [Fact]
    public void Update_WithDocumentType_KeepsIt()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", Cedula, true);
        var nit = new DocumentType(Guid.NewGuid(), "Número de Identificación Tributaria", "NIT");

        // Act
        user.Update("John", "Doe", "john.doe@example.com", "3105550131", null, "901888777-3", nit, true, Guid.NewGuid());

        // Assert
        Assert.Equal(nit, user.DocumentType);
    }
}
