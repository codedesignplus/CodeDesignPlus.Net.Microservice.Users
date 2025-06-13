using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddRole;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.AddTenant;
using CodeDesignPlus.Net.Microservice.Users.Application.User.Queries.GetUsersById;
using Google.Protobuf.WellKnownTypes;

namespace CodeDesignPlus.Net.Microservice.Users.gRpc.Services;

public class UserService(IMediator mediator) : Users.UsersBase
{
    public override async Task<Empty> AddGroupToUser(AddGroupRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out Guid id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));

        var user = await mediator.Send(new GetUsersByIdQuery(id), context.CancellationToken);

        if (user.Roles.Contains(request.Role))
            return new Empty();

        var command = new AddRoleCommand(id, request.Role);

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
}