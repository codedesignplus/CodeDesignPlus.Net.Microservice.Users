using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Dtos;
using CodeDesignPlus.Net.Microservice.Users.Domain;
using CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;
using CodeDesignPlus.Net.PubSub.Abstractions;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Users.AsyncWorker.Test.Consumers;

/// <summary>
/// Cubre el aprovisionamiento del comprador cuando se paga una licencia.
/// </summary>
/// <remarks>
/// Los dos comandos leen el mismo usuario, cambian una lista distinta y reescriben el documento entero.
/// Lanzados a la vez leian el mismo estado de partida y el ultimo en guardar borraba lo del otro. Paso en una
/// compra real el 2026-08-19: el rol quedo grabado, la copropiedad no, y el comprador no veia nada al entrar
/// pese a que los dos eventos se habian publicado sin error.
/// </remarks>
public class CompleteOrderHandlerTest
{
    private static readonly Guid Comprador = Guid.Parse("58869df7-18a5-4980-af64-b37d57843240");
    private static readonly Guid Copropiedad = Guid.Parse("5ba0755d-5c05-48c1-baa8-36154eacb74f");
    private static readonly Guid Otra = Guid.Parse("cac3ecb2-519b-4619-bd96-230118fcc182");

    /// <summary>Imita el documento del usuario: leer, cambiar y reescribirlo entero.</summary>
    private sealed class Documento
    {
        public List<string> Roles { get; set; } = [];
        public List<Guid> Tenants { get; set; } = [];
    }

    [Fact]
    public async Task LaCopropiedadNoSePideHastaQueElRolTermina()
    {
        // Es la prueba que fija el arreglo. Los dos comandos reescriben el documento entero del usuario, asi
        // que el segundo tiene que leer lo que dejo el primero. Lanzarlos a la vez hacia que ambos partieran
        // del mismo estado y el ultimo en guardar borrara lo del otro.
        var rolEnCurso = new TaskCompletionSource();
        var tenantPedido = false;

        var mediator = new Mock<IMediator>();

        mediator
            .Setup(x => x.Send(It.IsAny<AddRoleCommand>(), It.IsAny<CancellationToken>()))
            .Returns(rolEnCurso.Task);

        mediator
            .Setup(x => x.Send(It.IsAny<AddTenantCommand>(), It.IsAny<CancellationToken>()))
            .Returns(() => { tenantPedido = true; return Task.CompletedTask; });

        var repositorio = new Mock<IUserRepository>();
        repositorio
            .Setup(x => x.ExistsAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CompleteOrderHandler(
            mediator.Object, repositorio.Object, new Mock<IPubSub>().Object,
            new Mock<ILogger<CompleteOrderHandler>>().Object);

        var enCurso = handler.HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.False(tenantPedido, "la copropiedad se pidio antes de que el rol terminara");

        rolEnCurso.SetResult();
        await enCurso;

        Assert.True(tenantPedido);
    }

    [Fact]
    public async Task ElCompradorSeQuedaConSuRolYConSuCopropiedad()
    {
        var doc = new Documento();

        await Handler(doc).HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.Equal(["Administrador"], doc.Roles);
        Assert.Equal([Copropiedad], doc.Tenants);
    }

    [Fact]
    public async Task UnaSegundaCompraDelMismoAdministradorNoPierdeLaPrimera()
    {
        // Es el caso que importa de verdad: el mismo administrador comprando licencia para otra copropiedad.
        // Ya tiene el rol y ya tiene una copropiedad, y nada de eso puede hacer que falle.
        var doc = new Documento { Roles = ["Administrador"], Tenants = [Otra] };

        await Handler(doc).HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.Equal(["Administrador"], doc.Roles);
        Assert.Equal([Otra, Copropiedad], doc.Tenants);
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
            Mediator(doc), repositorio.Object, pubsub.Object,
            new Mock<ILogger<CompleteOrderHandler>>().Object);

        await handler.HandleAsync(Evento(Copropiedad), CancellationToken.None);

        Assert.Empty(doc.Roles);
        Assert.Empty(doc.Tenants);
    }

    private static CompleteOrderHandler Handler(Documento doc)
    {
        var repositorio = new Mock<IUserRepository>();
        repositorio
            .Setup(x => x.ExistsAsync<UserAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return new CompleteOrderHandler(
            Mediator(doc), repositorio.Object, new Mock<IPubSub>().Object,
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
                var roles = doc.Roles.ToList();
                var tenants = doc.Tenants.ToList();
                await Task.Yield();
                if (!roles.Contains(c.Role)) roles.Add(c.Role);
                doc.Roles = roles;
                doc.Tenants = tenants;
            });

        mediator
            .Setup(x => x.Send(It.IsAny<AddTenantCommand>(), It.IsAny<CancellationToken>()))
            .Returns(async (AddTenantCommand c, CancellationToken _) =>
            {
                var roles = doc.Roles.ToList();
                var tenants = doc.Tenants.ToList();
                await Task.Yield();
                if (!tenants.Contains(c.Tenant.Id)) tenants.Add(c.Tenant.Id);
                doc.Roles = roles;
                doc.Tenants = tenants;
            });

        return mediator.Object;
    }

    private static OrderPaidAndReadyForProvisioningDomainEvent Evento(Guid tenantId)
        => new(Guid.NewGuid(), new Tenant { Id = tenantId, Name = "Malpelo XVI" }, Comprador);
}
