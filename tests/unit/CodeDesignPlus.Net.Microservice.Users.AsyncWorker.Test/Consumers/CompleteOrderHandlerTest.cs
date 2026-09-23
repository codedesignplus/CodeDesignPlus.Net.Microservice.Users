using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.Options;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Dtos;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using CodeDesignPlus.Net.PubSub.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Test.Consumers;

/// <summary>
/// Cubre el aprovisionamiento del comprador cuando se paga una licencia.
/// </summary>
/// <remarks>
/// Los dos comandos leen el mismo usuario, cambian una lista distinta y reescriben el documento entero.
/// Lanzados a la vez leian el mismo estado de partida y el ultimo en guardar borraba lo del otro. Paso en una
/// compra real el 2026-08-19: el rol quedo grabado, la copropiedad no, y el comprador no veia nada al entrar
/// pese a que los dos eventos se habian publicado sin error.
/// <para>
/// Desde que el rol cuelga de la copropiedad, el orden ademas <b>importa por si mismo</b>: darle un papel a
/// alguien en una copropiedad a la que todavia no pertenece se rechaza.
/// </para>
/// </remarks>
public class CompleteOrderHandlerTest
{
    private static readonly Guid Comprador = Guid.Parse("58869df7-18a5-4980-af64-b37d57843240");
    private static readonly Guid Copropiedad = Guid.Parse("5ba0755d-5c05-48c1-baa8-36154eacb74f");
    private static readonly Guid Otra = Guid.Parse("cac3ecb2-519b-4619-bd96-230118fcc182");
    private static readonly Guid Administrador = Guid.Parse("1a43656c-f457-4695-8bfd-903be4b66097");

    /// <summary>Imita el documento del usuario: leer, cambiar y reescribirlo entero.</summary>
    private sealed class Documento
    {
        public Dictionary<Guid, List<Guid>> Roles { get; set; } = [];
        public List<Guid> Tenants { get; set; } = [];
    }

    [Fact]
    public async Task ElRolNoSePideHastaQueLaCopropiedadEsta()
    {
        // Es la prueba que fija el orden, y ahora por dos razones. La primera es la de siempre: los dos
        // comandos reescriben el documento entero, asi que el segundo tiene que leer lo que dejo el primero.
        // La segunda es nueva: un rol cuelga de una copropiedad, asi que pedirlo antes de que el usuario
        // pertenezca a ella es pedir un papel en ninguna parte.
        var tenantEnCurso = new TaskCompletionSource();
        var rolPedido = false;

        var mediator = new Mock<IMediator>();

        mediator
            .Setup(x => x.Send(It.IsAny<AddTenantCommand>(), It.IsAny<CancellationToken>()))
            .Returns(tenantEnCurso.Task);

        mediator
            .Setup(x => x.Send(It.IsAny<AddRoleCommand>(), It.IsAny<CancellationToken>()))
            .Returns(() => { rolPedido = true; return Task.CompletedTask; });

        var repositorio = new Mock<IUserRepository>();
        repositorio
            .Setup(x => x.ExistsAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CompleteOrderHandler(
            mediator.Object, repositorio.Object, new Mock<IPubSub>().Object, Opciones(),
            new Mock<ILogger<CompleteOrderHandler>>().Object);

        var enCurso = handler.HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.False(rolPedido, "el rol se pidio antes de que la copropiedad estuviera");

        tenantEnCurso.SetResult();
        await enCurso;

        Assert.True(rolPedido);
    }

    [Fact]
    public async Task ElCompradorSeQuedaConSuRolYConSuCopropiedad()
    {
        var doc = new Documento();

        await Handler(doc).HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.Equal([Copropiedad], doc.Tenants);
        Assert.Equal([Administrador], doc.Roles[Copropiedad]);
    }

    [Fact]
    public async Task UnaSegundaCompraDelMismoAdministradorNoPierdeLaPrimera()
    {
        // Es el caso que importa de verdad: el mismo administrador comprando licencia para otra copropiedad.
        // Ya tiene el rol en la primera y nada de eso puede hacer que falle.
        var doc = new Documento
        {
            Roles = new Dictionary<Guid, List<Guid>> { [Otra] = [Administrador] },
            Tenants = [Otra],
        };

        await Handler(doc).HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.Equal([Otra, Copropiedad], doc.Tenants);

        // Y ahora el papel se le da en cada una por separado, que es justo lo que antes no se podia decir.
        Assert.Equal([Administrador], doc.Roles[Otra]);
        Assert.Equal([Administrador], doc.Roles[Copropiedad]);
    }

    [Fact]
    public async Task SiElCompradorNoExisteSeAvisaEnVezDeSeguir()
    {
        var repositorio = new Mock<IUserRepository>();
        repositorio
            .Setup(x => x.ExistsAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var doc = new Documento();
        var pubsub = new Mock<IPubSub>();

        var handler = new CompleteOrderHandler(
            Mediator(doc), repositorio.Object, pubsub.Object, Opciones(),
            new Mock<ILogger<CompleteOrderHandler>>().Object);

        await handler.HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.Empty(doc.Roles);
        Assert.Empty(doc.Tenants);
    }

    private static IOptions<RoleAssignmentOptions> Opciones() =>
        Microsoft.Extensions.Options.Options.Create(new RoleAssignmentOptions { AdministrationRole = Administrador });

    private static CompleteOrderHandler Handler(Documento doc)
    {
        var repositorio = new Mock<IUserRepository>();
        repositorio
            .Setup(x => x.ExistsAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return new CompleteOrderHandler(
            Mediator(doc), repositorio.Object, new Mock<IPubSub>().Object, Opciones(),
            new Mock<ILogger<CompleteOrderHandler>>().Object);
    }

    /// <summary>
    /// Cada comando lee el documento, cede el turno y luego lo reescribe entero. El <c>Yield</c> es lo que
    /// hace visible la perdida: en paralelo los dos leen el mismo estado y el segundo pisa al primero.
    /// </summary>
    private static IMediator Mediator(Documento doc)
    {
        var mediator = new Mock<IMediator>();

        mediator
            .Setup(x => x.Send(It.IsAny<AddRoleCommand>(), It.IsAny<CancellationToken>()))
            .Returns(async (AddRoleCommand c, CancellationToken _) =>
            {
                var roles = doc.Roles.ToDictionary(x => x.Key, x => x.Value.ToList());
                var tenants = doc.Tenants.ToList();
                await Task.Yield();

                if (!tenants.Contains(c.TenantId))
                    return;

                if (!roles.TryGetValue(c.TenantId, out var deLaCopropiedad))
                    roles[c.TenantId] = deLaCopropiedad = [];

                if (!deLaCopropiedad.Contains(c.Role))
                    deLaCopropiedad.Add(c.Role);

                doc.Roles = roles;
                doc.Tenants = tenants;
            });

        mediator
            .Setup(x => x.Send(It.IsAny<AddTenantCommand>(), It.IsAny<CancellationToken>()))
            .Returns(async (AddTenantCommand c, CancellationToken _) =>
            {
                var roles = doc.Roles.ToDictionary(x => x.Key, x => x.Value.ToList());
                var tenants = doc.Tenants.ToList();
                await Task.Yield();

                if (!tenants.Contains(c.Tenant.Id))
                    tenants.Add(c.Tenant.Id);

                doc.Roles = roles;
                doc.Tenants = tenants;
            });

        return mediator.Object;
    }

    private static OrderPaidAndReadyForProvisioningDomainEvent Evento(Guid tenantId)
        => new(Guid.NewGuid(), new Tenant { Id = tenantId, Name = "Malpelo XVI" }, Comprador);
}
