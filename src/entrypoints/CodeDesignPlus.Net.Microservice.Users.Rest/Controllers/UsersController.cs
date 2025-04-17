using CodeDesignPlus.Net.Microservice.Users.Application.Users.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Users.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator, IMapper mapper) : ControllerBase
{
     /// <summary>
    /// Get all Users.
    /// </summary>
    /// <param name="criteria">Criteria for filtering and sorting the Users.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of Users.</returns>
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] C.Criteria criteria, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllUsersQuery(criteria), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a User by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The User.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create a new User.
    /// </summary>
    /// <param name="data">Data for creating the User.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<CreateUserCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update an existing User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">Data for updating the User.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateUserCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(id), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Add a Tenant to a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">Data for adding the Tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost("{id}/tenant")]
    public async Task<IActionResult> AddTenantToUser(Guid id, [FromBody] TenantDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(new AddTenantCommand(id, data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Remove a Tenant from a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="tenantId">The unique identifier of the Tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}/tenant/{tenantId}")]
    public async Task<IActionResult> RemoveTenantFromUser(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveTenantCommand(id, tenantId), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Add a Role to a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="role">The role to be added.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost("{id}/role")]
    public async Task<IActionResult> AddRoleToUser(Guid id, [FromBody] string role, CancellationToken cancellationToken)
    {
        await mediator.Send(new AddRoleCommand(id, role), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Remove a Role from a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="role">The role to be removed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}/role/{role}")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid id, string role, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveRoleCommand(id, role), cancellationToken);

        return NoContent();
    }
}
