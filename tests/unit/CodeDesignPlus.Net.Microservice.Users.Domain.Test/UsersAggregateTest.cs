using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;
using NodaTime;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.Test;

public class UserAggregateTest
{
    [Fact]
    public void Create_ShouldInitializeUserAggregate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phone = "1234567890";
        var displayName = "John Doe";
        var createdBy = Guid.NewGuid();

        var documentNumber = "CC123456";
        var documentType = new DocumentType(Guid.NewGuid(), "Cédula de Ciudadanía", "CC");

        // Act
        var user = UserAggregate.Create(id, firstName, lastName, email, phone, displayName, documentNumber, documentType, true);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(phone, user.Phone);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Equal(documentNumber, user.DocumentNumber);
        Assert.Equal(documentType, user.DocumentType);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Update_ShouldUpdateUserDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);
        var updatedBy = Guid.NewGuid();

        // Act
        user.Update("Jane", "Smith", "jane.smith@example.com", "0987654321", "Jane Smith", "9876543210", null, false, updatedBy);

        // Assert
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Smith", user.LastName);
        Assert.Equal("jane.smith@example.com", user.Email);
        Assert.Equal("0987654321", user.Phone);
        Assert.Equal("Jane Smith", user.DisplayName);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void AddTenant_ShouldAddTenantToUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);
        var tenantId = Guid.NewGuid();
        var tenantName = "Tenant1";
        var updatedBy = Guid.NewGuid();

        // Act
        user.AddTenant(tenantId, tenantName, updatedBy);

        // Assert
        Assert.Single(user.Tenants);
        Assert.Equal(tenantId, user.Tenants.First().Id);
        Assert.Equal(tenantName, user.Tenants.First().Name);
    }

    [Fact]
    public void RemoveTenant_ShouldRemoveTenantFromUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);
        var tenantId = Guid.NewGuid();
        user.AddTenant(tenantId, "Tenant1", Guid.NewGuid());

        // Act
        user.RemoveTenant(tenantId, Guid.NewGuid());

        // Assert
        Assert.Empty(user.Tenants);
    }

    [Fact]
    public void AddRole_ShouldAddRoleToUser()
    {
        // Arrange
        var user = ConUnaCopropiedad(out var tenantId);
        var role = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();

        // Act
        user.AddRole(tenantId, role, updatedBy);

        // Assert
        Assert.Contains(role, user.Tenants.Single(x => x.Id == tenantId).Roles);
    }

    [Fact]
    public void RemoveRole_ShouldRemoveRoleFromUser()
    {
        // Arrange
        var user = ConUnaCopropiedad(out var tenantId);
        var role = Guid.NewGuid();
        user.AddRole(tenantId, role, Guid.NewGuid());

        // Act
        user.RemoveRole(tenantId, role, Guid.NewGuid());

        // Assert
        Assert.DoesNotContain(role, user.Tenants.Single(x => x.Id == tenantId).Roles);
    }

    [Fact]
    public void AddRole_EnUnaCopropiedadQueNoEsSuya_Falla()
    {
        // Arrange: un rol sin copropiedad no existe, y darlo en una a la que el usuario no pertenece
        // seria darselo en ninguna parte.
        var user = ConUnaCopropiedad(out _);

        // Act
        var error = Assert.Throws<CodeDesignPlusException>(
            () => user.AddRole(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));

        // Assert
        Assert.Contains("109", error.Code);
    }

    [Fact]
    public void RemoveRole_EnUnaCopropiedad_NoLoQuitaDeLasDemas()
    {
        // Arrange
        var user = ConUnaCopropiedad(out var primera);
        var segunda = Guid.NewGuid();
        user.AddTenant(segunda, "Malpelo XXII", Guid.NewGuid());

        var role = Guid.NewGuid();
        user.AddRole(primera, role, Guid.NewGuid());
        user.AddRole(segunda, role, Guid.NewGuid());

        // Act
        user.RemoveRole(primera, role, Guid.NewGuid());

        // Assert: este es el detalle que se olvida. Quitar el papel en una copropiedad no puede
        // quitarlo en las otras, ni sacar al usuario del grupo del proveedor de identidad.
        Assert.DoesNotContain(role, user.Tenants.Single(x => x.Id == primera).Roles);
        Assert.Contains(role, user.Tenants.Single(x => x.Id == segunda).Roles);

        // Y el evento tiene que decirlo, porque es lo unico que impide que el consumidor lo saque del
        // grupo del proveedor de identidad, que es global.
        var aviso = user.GetAndClearEvents().OfType<RoleRemovedToUserDomainEvent>().Single();

        Assert.True(aviso.StillHasItElsewhere);
    }

    private static UserAggregate ConUnaCopropiedad(out Guid tenantId)
    {
        tenantId = Guid.NewGuid();

        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);

        user.AddTenant(tenantId, "Malpelo XXI", Guid.NewGuid());

        return user;
    }

    [Fact]
    public void UpdateContactInfo_ShouldUpdateContactDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);
        var updatedBy = Guid.NewGuid();

        // Act
        user.UpdateContactInfo("123 Street", "City", "State", "Country", "12345", "1234567890", ["contact@example.com"], updatedBy);

        // Assert
        Assert.Equal("123 Street", user.Contact.Address);
        Assert.Equal("City", user.Contact.City);
    }

    [Fact]
    public void UpdateJobInfo_ShouldUpdateJobDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);
        var updatedBy = Guid.NewGuid();

        // Act
        user.UpdateJobInfo("Developer", "Company", "IT", "123", "Full-Time", SystemClock.Instance.GetCurrentInstant(), "Office", updatedBy);

        // Assert
        Assert.Equal("Developer", user.Job.JobTitle);
        Assert.Equal("Company", user.Job.CompanyName);
    }

    [Fact]
    public void RemovePurgedTenant_WithRoles_RemovesTheTenantAndAnnouncesEveryRole()
    {
        // Arrange
        var purged = Guid.NewGuid();
        var other = Guid.NewGuid();
        var administrator = Guid.NewGuid();
        var resident = Guid.NewGuid();
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);

        user.AddTenant(purged, "Malpelo XXI", Guid.NewGuid());
        user.AddTenant(other, "Malpelo VI", Guid.NewGuid());
        user.AddRole(purged, administrator, Guid.NewGuid());
        user.AddRole(purged, resident, Guid.NewGuid());
        user.AddRole(other, resident, Guid.NewGuid());
        user.GetAndClearEvents();

        // Act
        user.RemovePurgedTenant(purged);

        // Assert
        var events = user.GetAndClearEvents();
        var removedRoles = events.OfType<RoleRemovedToUserDomainEvent>().ToDictionary(x => x.Role);

        Assert.DoesNotContain(user.Tenants, x => x.Id == purged);
        Assert.Equal(2, removedRoles.Count);
        Assert.False(removedRoles[administrator].StillHasItElsewhere);
        Assert.True(removedRoles[resident].StillHasItElsewhere);
        Assert.Equal(purged, events.OfType<TenantRemovedDomainEvent>().Single().Tenant.Id);
    }

    [Fact]
    public void RemovePurgedTenant_NotAMember_ThrowsTenantNotFound()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => user.RemovePurgedTenant(Guid.NewGuid()));

        // Assert
        Assert.Equal(Errors.TenantNotFound.GetCode(), exception.Code);
    }
}
