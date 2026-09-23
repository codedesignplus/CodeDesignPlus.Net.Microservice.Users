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
    public string DocumentNumber { get; private set; } = null!;
    public DocumentType? DocumentType { get; private set; }
    public List<TenantEntity> Tenants { get; private set; } = [];

    /// <summary>
    /// Los roles de plataforma: los unicos que no cuelgan de ninguna copropiedad.
    /// </summary>
    /// <remarks>
    /// Los roles de copropiedad viven en <see cref="TenantEntity.Roles"/>. Aqui solo queda lo que se es
    /// en Kappali entera, como quien administra la plataforma: un papel que no depende de que
    /// copropiedad se este mirando.
    /// </remarks>
    public string[] Roles { get; private set; } =  [];
    public ContactInfo Contact { get; private set; } = null!;
    public JobInfo Job { get; private set; } = null!;

    public UserAggregate(Guid id, string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, DocumentType? documentType, bool isActive) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdUserIsRequired);
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneRequired);
        DomainGuard.IsNullOrEmpty(documentNumber, Errors.DocumentNumberRequired);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName ?? $"{firstName} {lastName}";
        DocumentNumber = documentNumber;
        DocumentType = documentType;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        IsActive = isActive;

        this.AddEvent(UserRegisteredDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, DocumentNumber, DocumentType, IsActive));
    }

    public static UserAggregate Create(Guid id, string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, DocumentType? documentType, bool isActive)
    {
        return new UserAggregate(id, firstName, lastName, email, phone, displayName, documentNumber, documentType, isActive);
    }

    public void Update(string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, DocumentType? documentType, bool isActive, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneRequired);
        DomainGuard.IsNullOrEmpty(documentNumber, Errors.DocumentNumberRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        DocumentNumber = documentNumber;
        DocumentType = documentType;
        IsActive = isActive;
        Picture = Picture;
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(UserUpdatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, DocumentNumber, DocumentType, IsActive));
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

        this.AddEvent(TenantAddedDomainEvent.Create(Id, DisplayName, Email, tenant));
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
    /// <summary>
    /// Anade un rol al usuario en una copropiedad concreta.
    /// </summary>
    /// <remarks>
    /// <b>Un rol sin copropiedad no existe.</b> El guard de pertenencia es la mitad del arreglo: hasta
    /// ahora no habia con que comprobar si el usuario pertenecia a la copropiedad en la que se le estaba
    /// dando un papel, porque la copropiedad no se pedia.
    /// </remarks>
    /// <param name="tenantId">La copropiedad en la que tendra el rol.</param>
    /// <param name="role">El id del grupo del proveedor de identidad.</param>
    /// <param name="updatedBy">Quien lo asigna.</param>
    public void AddRole(Guid tenantId, Guid role, Guid updatedBy)
    {
        DomainGuard.GuidIsEmpty(role, Errors.RolesRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);

        var tenant = Tenants.FirstOrDefault(t => t.Id == tenantId);

        DomainGuard.IsNull(tenant, Errors.TenantNotFound);
        DomainGuard.IsTrue(tenant.Roles.Contains(role), Errors.RoleAlreadyExists);

        tenant.Roles.Add(role);
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(RoleAddedToUserDomainEvent.Create(Id, DisplayName, tenantId, role));
    }
    /// <summary>
    /// Retira un rol del usuario en una copropiedad concreta.
    /// </summary>
    /// <remarks>
    /// Retirarlo de una no lo retira de las demas. Quien lo consuma tiene que tenerlo presente antes de
    /// sacar al usuario del grupo del proveedor de identidad, que si es global.
    /// </remarks>
    /// <param name="tenantId">La copropiedad de la que se retira.</param>
    /// <param name="role">El id del grupo del proveedor de identidad.</param>
    /// <param name="updateBy">Quien lo retira.</param>
    public void RemoveRole(Guid tenantId, Guid role, Guid updateBy)
    {
        DomainGuard.GuidIsEmpty(role, Errors.RolesRequired);

        var tenant = Tenants.FirstOrDefault(t => t.Id == tenantId);

        DomainGuard.IsNull(tenant, Errors.TenantNotFound);
        DomainGuard.IsFalse(tenant.Roles.Contains(role), Errors.RoleNotFound);

        tenant.Roles.Remove(role);

        UpdatedBy = updateBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        var leQuedaEnOtra = Tenants.Exists(x => x.Id != tenantId && x.Roles.Contains(role));

        this.AddEvent(RoleRemovedToUserDomainEvent.Create(Id, DisplayName, tenantId, role, leQuedaEnOtra));
    }

    public void Delete(Guid deletedBy)
    {
        DomainGuard.GuidIsEmpty(deletedBy, Errors.IdUserIsRequired);

        this.IsDeleted = true;
        this.IsActive = false;
        this.DeletedAt = SystemClock.Instance.GetCurrentInstant();
        this.DeletedBy = deletedBy;

        this.AddEvent(UserDeletedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, DocumentNumber, DocumentType, IsActive));
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

    public void UpdateProfile(string firstName, string lastName, string email, string phone, string? displayName, string documentNumber, DocumentType? documentType, bool isActive, ContactInfo contact, JobInfo job, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(firstName, Errors.FirstNameRequired);
        DomainGuard.IsNullOrEmpty(lastName, Errors.LastNameRequired);
        DomainGuard.IsNullOrEmpty(email, Errors.EmailRequired);
        DomainGuard.IsNullOrEmpty(phone, Errors.PhoneRequired);
        DomainGuard.IsNullOrEmpty(documentNumber, Errors.DocumentNumberRequired);
        DomainGuard.GuidIsEmpty(updatedBy, Errors.UpdateByInvalid);

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        DocumentNumber = documentNumber;
        DocumentType = documentType;
        IsActive = isActive;
        Contact = contact;
        Job = job;
        UpdatedBy = updatedBy;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(ProfileUpdatedDomainEvent.Create(Id, FirstName, LastName, Email, Phone, DisplayName, DocumentNumber, DocumentType, IsActive, Contact, Job));
    }
}
