using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;
using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Domain;

public class UserAggregate(Guid id) : AggregateRootBase(id)
{
    public UserPicture? Picture { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? DisplayName { get; private set; } = null!;
    public List<TenantEntity> Tenants { get; private set; } = [];
    public string[] Roles { get; private set; } =  [];
    public ContactInfo Contact { get; private set; } = null!;
    public JobInfo Job { get; private set; } = null!;

    public UserAggregate(Guid id, string firstName, string lastName, string email, string phone, string? displayName, Guid createdBy) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdUserIsRequired);
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneRequired);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName ?? $"{firstName} {lastName}";

        CreatedBy = createdBy;
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        IsActive = true;

        //this.AddEvent(UserCreatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive));
    }

    public static UserAggregate Create(Guid id, string firstName, string lastName, string email, string phone, string? displayName, Guid createdBy)
    {
        return new UserAggregate(id, firstName, lastName, email, phone, displayName, createdBy);
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
        Picture = Picture;
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(UserUpdatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive));
    }

    public void UpdatePicture(Guid id, string name, string target, Guid updatedBy)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdUserIsRequired);
        DomainGuard.IsNullOrEmpty(name, Errors.ImageRequired);
        DomainGuard.IsNullOrEmpty(target, Errors.ImageRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);
        
        Picture = UserPicture.Create(id, name, target);;

        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(UserPictureUpdatedDomainEvent.Create(Id, Picture.Name, Picture.Target));
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

        this.AddEvent(TenantAddedDomainEvent.Create(Id, DisplayName, tenant));
    }
    public void RemoveTenant(Guid tenantId, Guid updateBy)
    {
        DomainGuard.GuidIsEmpty(tenantId, Errors.IdUserIsRequired);

        var tenant = Tenants.FirstOrDefault(t => t.Id == tenantId);

        DomainGuard.IsNull(tenant, Errors.TenantNotFound);

        Tenants.Remove(tenant);
        UpdatedBy = updateBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(TenantRemovedDomainEvent.Create(Id, DisplayName, tenant));
    }
    public void AddRole(string role, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(role, Errors.RolesRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);

        DomainGuard.IsTrue(Roles.Any(r => r == role), Errors.RoleAlreadyExists);

        Roles = [.. Roles, role];
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(RoleAddedToUserDomainEvent.Create(Id, DisplayName, role));
    }
    public void RemoveRole(string role, Guid updateBy)
    {
        DomainGuard.IsNullOrEmpty(role, Errors.RolesRequired);

        var item = Roles.FirstOrDefault(r => r == role);

        DomainGuard.IsNull(item, Errors.RoleNotFound);

        Roles = [.. Roles.Where(r => r != role)];

        UpdatedBy = updateBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(RoleRemovedToUserDomainEvent.Create(Id, DisplayName, role));
    }

    public void Delete(Guid idUser)
    {
        DomainGuard.GuidIsEmpty(idUser, Errors.IdUserIsRequired);

        this.AddEvent(UserDeletedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive));
    }

    public void UpdateContactInfo(string address, string city, string state, string country, string postalCode, string phone, string[] email, Guid updatedBy)
    {
        Contact = ContactInfo.Create(address, city, state, country, postalCode, phone, email);

        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(ContactInfoUpdatedDomainEvent.Create(Id, Contact));
    }

    public void UpdateJobInfo(string jobTitle, string companyName, string department, string employeeId, string employeeType, Instant employHireDate, string officeLocation, Guid updatedBy)
    {
        Job = JobInfo.Create(jobTitle, companyName, department, employeeId, employeeType, employHireDate, officeLocation);

        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(JobInfoUpdatedDomainEvent.Create(Id, Job));
    }

    public void UpdateProfile(string firstName, string lastName, string email, string phone, string? displayName, bool isActive, ContactInfo contact, JobInfo job, Guid updatedBy)
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
        Contact = contact;
        Job = job;
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(ProfileUpdatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, IsActive, Contact, Job));
    }
}
