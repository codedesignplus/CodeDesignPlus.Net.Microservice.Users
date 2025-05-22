using System.Text.Json.Serialization;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

public sealed partial class UserPicture
{
    public Guid Id { get; private set; } 
    public string Name { get; private set; }
    public string Target { get; private set; }

    public UserPicture()
    {
        this.Id = Guid.Empty;
        this.Name = null!;
        this.Target = null!;
    }

    [JsonConstructor]
    public UserPicture(Guid id, string name, string target)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdUserIsRequired);
        DomainGuard.IsNullOrEmpty(name, Errors.UnknownError);
        DomainGuard.IsNullOrEmpty(target, Errors.UnknownError);

        this.Id = id;
        this.Name = name;
        this.Target = target;
    }
    public static UserPicture Create(Guid id, string name, string target)
    {
        return new UserPicture(id, name, target);
    }
}
