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

        // Act
        var user = UserAggregate.Create(id, firstName, lastName, email, phone, displayName, true, createdBy);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(phone, user.Phone);
        Assert.Equal(displayName, user.DisplayName);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Update_ShouldUpdateUserDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
        var updatedBy = Guid.NewGuid();

        // Act
        user.Update("Jane", "Smith", "jane.smith@example.com", "0987654321", "Jane Smith", false, updatedBy);

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
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
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
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
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
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
        var role = "Admin";
        var updatedBy = Guid.NewGuid();

        // Act
        user.AddRole(role, updatedBy);

        // Assert
        Assert.Contains(role, user.Roles);
    }

    [Fact]
    public void RemoveRole_ShouldRemoveRoleFromUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
        var role = "Admin";
        user.AddRole(role, Guid.NewGuid());

        // Act
        user.RemoveRole(role, Guid.NewGuid());

        // Assert
        Assert.DoesNotContain(role, user.Roles);
    }

    [Fact]
    public void UpdateContactInfo_ShouldUpdateContactDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
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
        var user = UserAggregate.Create(id, "John", "Doe", "john.doe@example.com", "1234567890", null, true, Guid.NewGuid());
        var updatedBy = Guid.NewGuid();

        // Act
        user.UpdateJobInfo("Developer", "Company", "IT", "123", "Full-Time", SystemClock.Instance.GetCurrentInstant(), "Office", updatedBy);

        // Assert
        Assert.Equal("Developer", user.Job.JobTitle);
        Assert.Equal("Company", user.Job.CompanyName);
    }
}