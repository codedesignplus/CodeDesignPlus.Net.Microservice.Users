using System.Linq;
using CodeDesignPlus.Net.Core.Abstractions.Contracts;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.Test;

/// <summary>
/// La foto que un usuario deja de usar se le avisa a ms-filestorage, en el espacio de la plataforma (pendings/172).
/// </summary>
public class PictureReleaseTest
{
    private static readonly Guid UpdatedBy = Guid.NewGuid();

    private static UserAggregate UserWithPicture(Guid picture)
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);
        user.UpdatePicture(picture, "john.jpg", "users", UpdatedBy);
        user.GetAndClearEvents();

        return user;
    }

    private static FilesReleasedDomainEvent? Released(UserAggregate user) =>
        user.GetAndClearEvents().OfType<FilesReleasedDomainEvent>().SingleOrDefault();

    [Fact]
    public void UpdatePicture_NewPicture_ReleasesTheOldOneInThePlatform()
    {
        var old = Guid.NewGuid();
        var user = UserWithPicture(old);

        user.UpdatePicture(Guid.NewGuid(), "john-2.jpg", "users", UpdatedBy);

        var released = Released(user);
        Assert.NotNull(released);
        Assert.Equal([old], released.Files);
        Assert.Equal(Guid.Empty, released.Tenant);
        Assert.Equal(UpdatedBy, released.ReleasedBy);
    }

    [Fact]
    public void UpdatePicture_FirstPicture_ReleasesNothing()
    {
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "1234567890", null, "1234567890", null, true);

        user.UpdatePicture(Guid.NewGuid(), "john.jpg", "users", UpdatedBy);

        Assert.Null(Released(user));
    }

    [Fact]
    public void Delete_UserWithPicture_ReleasesIt()
    {
        var picture = Guid.NewGuid();
        var user = UserWithPicture(picture);

        user.Delete(UpdatedBy);

        Assert.Equal([picture], Released(user)?.Files);
    }
}
