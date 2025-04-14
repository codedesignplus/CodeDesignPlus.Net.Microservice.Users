using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Domain;

public class UsersAggregate(Guid id) : AggregateRootBase(id)
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? DisplayName { get; private set; } = null!;
    public List<TenantEntity> Tenants { get; private set; } = [];
    public string[] Roles { get; private set; } = null!;

    public UsersAggregate(Guid id, string firstName, string lastName, string email, string phone, string displayName, string[] roles) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdUserIsRequired);
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneRequired);
        DomainGuard.IsEmpty(roles, Errors.RolesRequired);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName ?? $"{firstName} {lastName}";
        Roles = roles;

        UserCreatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive);
    }

    public static UsersAggregate Create(Guid id, string firstName, string lastName, string email, string phone, string? displayName, string[] roles)
    {
        return new UsersAggregate(id)
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DisplayName = displayName,
            Roles = roles,
            IsActive = true,
            CreatedAt = SystemClock.Instance.GetCurrentInstant(),
        };
    }

    public void Update(string firstName, string lastName, string email, string phone, string? displayName, bool isActive, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        IsActive = isActive;
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        UsersUpdatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive);
    }

    public void AddTenant(Guid tenantId, string name, Guid updateBy)
    {
        DomainGuard.GuidIsEmpty(tenantId, Errors.IdUserIsRequired);
        DomainGuard.IsNullOrEmpty(name, Errors.FirstNameRequired);
        DomainGuard.IsTrue(Tenants.Any(t => t.Id == tenantId), Errors.TenantAlreadyExists);
        DomainGuard.GuidIsEmpty(updateBy, Errors.UpdateByInvalid);

        var tenant = new TenantEntity
        {
            Id = tenantId,
            Name = name
        };

        Tenants.Add(tenant);

        UpdatedBy = updateBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        TenantAddedDomainEvent.Create(Id, DisplayName, tenant);
    }
    public void RemoveTenant(Guid tenantId, Guid updateBy)
    {
        DomainGuard.GuidIsEmpty(tenantId, Errors.IdUserIsRequired);

        var tenant = Tenants.FirstOrDefault(t => t.Id == tenantId);

        DomainGuard.IsNull(tenant, Errors.TenantNotFound);

        Tenants.Remove(tenant);
        UpdatedBy = updateBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        TenantRemovedDomainEvent.Create(Id, DisplayName, tenant);
    }
    public void AddRole(string role, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(role, Errors.RolesRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);

        DomainGuard.IsTrue(Roles.Any(r => r == role), Errors.RoleAlreadyExists);

        Roles = [.. Roles, role];
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        RoleAddedDomainEvent.Create(Id, DisplayName, role);
    }
    public void RemoveRole(string role, Guid updateBy)
    {
        DomainGuard.IsNullOrEmpty(role, Errors.RolesRequired);

        var item = Roles.FirstOrDefault(r => r == role);

        DomainGuard.IsNull(item, Errors.RoleAlreadyExists);

        Roles = [.. Roles.Where(r => r != role)];

        UpdatedBy = updateBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
    }

    public void Delete(Guid idUser)
    {
        DomainGuard.GuidIsEmpty(idUser, Errors.IdUserIsRequired);

        UsersDeletedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive);
    }
}
