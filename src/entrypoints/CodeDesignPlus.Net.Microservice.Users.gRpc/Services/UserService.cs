using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.RemoveRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
using Google.Protobuf.WellKnownTypes;

namespace CodeDesignPlus.Net.Microservice.Users.gRpc.Services;

public class UserService(IMediator mediator) : Users.UsersBase
{
    public override async Task<Empty> AddGroupToUser(AddGroupRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));

        if (!Guid.TryParse(request.Tenant, out Guid tenantId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Tenant"));

        // El rol es el identificador del grupo del proveedor de identidad. Antes llegaba el nombre, y
        // comparar nombres contra lo que trae el token -que son identificadores- no casaba nunca.
        if (!Guid.TryParse(request.Role, out Guid role))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Role"));

        var user = await mediator.Send(new GetUsersByIdQuery(id), context.CancellationToken);

        var tenant = user.Tenants.FirstOrDefault(x => x.Id == tenantId);

        if (tenant is not null && tenant.Roles.Contains(role))
            return new Empty();

        var command = new AddRoleCommand(id, tenantId, role, Guid.NewGuid());

        await mediator.Send(command, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> RemoveGroupFromUser(RemoveGroupRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));

        if (!Guid.TryParse(request.Tenant, out Guid tenantId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Tenant"));

        if (!Guid.TryParse(request.Role, out Guid role))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Role"));

        var user = await mediator.Send(new GetUsersByIdQuery(id), context.CancellationToken);

        var tenant = user.Tenants.FirstOrDefault(x => x.Id == tenantId);

        // Se retira solo de esta copropiedad: en las demas el usuario conserva el rol.
        if (tenant is null || !tenant.Roles.Contains(role))
            return new Empty();

        var command = new RemoveRoleCommand(id, tenantId, role, Guid.NewGuid());

        await mediator.Send(command, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> AddTenantToUser(AddTenantRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));

        if (!Guid.TryParse(request.Tenant.Id, out Guid idTenant))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Tenant Id"));

        var user = await mediator.Send(new GetUsersByIdQuery(id), context.CancellationToken);

        if (user.Tenants.Any(x => x.Id == idTenant))
            return new Empty();


        var command = new AddTenantCommand(id, new TenantDto
        {
            Id = idTenant,
            Name = request.Tenant.Name,
        });

        await mediator.Send(command, context.CancellationToken);

        return new Empty();
    }

    /// <summary>
    /// Ultimo recurso del directorio de roles, cuando la cache compartida no tiene la instantanea.
    /// </summary>
    /// <remarks>
    /// Devuelve el mapa completo y no los roles de una copropiedad: asi un usuario que cambia de
    /// copropiedad en la misma sesion no provoca una llamada por cada cambio.
    /// </remarks>
    public override async Task<GetUserRolesResponse> GetUserRoles(GetUserRolesRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));

        var user = await mediator.Send(new GetUsersByIdQuery(id), context.CancellationToken);

        if (user is null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        var response = new GetUserRolesResponse();

        response.Platform.AddRange(user.Roles ?? []);

        foreach (var tenant in user.Tenants)
        {
            var roles = new TenantRoles { Tenant = tenant.Id.ToString() };

            roles.Roles.AddRange(tenant.Roles.Select(x => x.ToString()));

            response.Tenants.Add(roles);
        }

        return response;
    }
}