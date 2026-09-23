using CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Application.User.DataTransferObjects;

public class UserDto : IDtoBase
{
    public required Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? DisplayName { get; set; } = null!;
    public string DocumentNumber { get; set; } = null!;
    public DocumentType? DocumentType { get; set; }
    public List<TenantDto> Tenants { get; set; } = [];
    /// <summary>
    /// Los roles de plataforma. Los de copropiedad estan en <see cref="TenantDto.Roles"/>.
    /// </summary>
    public string[] Roles { get; set; } = null!;
    public ContactInfo Contact { get; set; } = null!;
    public JobInfo Job { get; set; } = null!;
    public UserPicture? Picture { get; set; } = null!;
    public bool IsActive { get; set; }
}