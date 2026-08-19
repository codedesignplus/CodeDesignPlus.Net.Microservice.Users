using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Domain.DomainEvents;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Test.User.Commands.AddRole;

/// <summary>
/// Cubre dos asignaciones de rol que caen a la vez sobre el mismo usuario.
/// </summary>
/// <remarks>
/// Registrar una titularidad y su morador dispara dos consumidores distintos —el rol de propietario y el de
/// residente— que llaman a la vez. Antes cada uno leia el usuario, le anadia su rol y guardaba el documento
/// entero: los dos partian del mismo estado y el ultimo en escribir borraba al otro. Ocurrio el 2026-08-19
/// con 59 ms de diferencia, y el propietario se quedo sin su rol de residente. Los eventos si se publicaron,
/// asi que el proveedor de identidad tenia los dos grupos y la base local solo uno.
/// </remarks>
public class AsignacionSimultaneaDeRolesTest
{
    private static readonly Guid Usuario = Guid.Parse("55024fb2-53cd-4ca9-b87d-694d67378182");

    /// <summary>Imita el documento del usuario: una lista que la base modifica sin releerla entera.</summary>
    private sealed class Documento
    {
        public List<string> Roles { get; } = [];
    }

    [Fact]
    public async Task LosDosRolesSobrevivenAunqueSeAsignenALaVez()
    {
        var doc = new Documento();
        var repositorio = Repositorio(doc);
        var pubsub = new Mock<IPubSub>();

        var handler = new AddRoleCommandHandler(repositorio.Object, pubsub.Object, Mock.Of<ICacheManager>());

        await Task.WhenAll(
            handler.Handle(new AddRoleCommand(Usuario, "Propietario", Usuario), CancellationToken.None),
            handler.Handle(new AddRoleCommand(Usuario, "Residente", Usuario), CancellationToken.None));

        Assert.Equal(2, doc.Roles.Count);
        Assert.Contains("Propietario", doc.Roles);
        Assert.Contains("Residente", doc.Roles);
    }

    [Fact]
    public async Task AsignarDosVecesElMismoRolNoLoDuplicaNiLoAnunciaDosVeces()
    {
        // Los consumidores reintentan, asi que la misma asignacion puede llegar mas de una vez. Anunciarla de
        // nuevo haria creer a quien escuche que acaba de pasar algo que ya habia pasado.
        var doc = new Documento();
        var repositorio = Repositorio(doc);
        var eventos = new List<IDomainEvent>();
        var pubsub = new Mock<IPubSub>();

        pubsub.Setup(x => x.PublishAsync(It.IsAny<IReadOnlyList<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyList<IDomainEvent>, CancellationToken>((e, _) => eventos.AddRange(e))
            .Returns(Task.CompletedTask);

        var handler = new AddRoleCommandHandler(repositorio.Object, pubsub.Object, Mock.Of<ICacheManager>());

        await handler.Handle(new AddRoleCommand(Usuario, "Propietario", Usuario), CancellationToken.None);
        await handler.Handle(new AddRoleCommand(Usuario, "Propietario", Usuario), CancellationToken.None);

        Assert.Single(doc.Roles);
        Assert.Single(eventos.OfType<RoleAddedToUserDomainEvent>());
    }

    /// <summary>
    /// El doble imita a <c>$addToSet</c>: anade sobre el estado real y no duplica. Ceder el turno antes de
    /// escribir es lo que hace visible la carrera — con un reemplazo del documento entero, el segundo en
    /// guardar borraria lo del primero.
    /// </summary>
    private static Mock<IUserRepository> Repositorio(Documento doc)
    {
        var repositorio = new Mock<IUserRepository>();

        repositorio
            .Setup(x => x.FindAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => UserAggregate.Create(Usuario, "Yago", "Valle", "yago@fake.com", "3100000000", "Yago Valle", "1022356894", null, true));

        repositorio
            .Setup(x => x.AddRoleAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(async (Guid _, string rol, Guid __, CancellationToken ___) =>
            {
                await Task.Yield();

                if (doc.Roles.Contains(rol))
                    return false;

                doc.Roles.Add(rol);
                return true;
            });

        return repositorio;
    }
}
