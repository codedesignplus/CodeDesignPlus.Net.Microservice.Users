using CodeDesignPlus.Net.Microservice.Users.Application.User.Commands.UpdatePicture;

namespace CodeDesignPlus.Net.Microservice.Users.Rest.Controllers;

/// <summary>
/// Controller for managing the Users.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and commands/queries.</param>
[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator, IMapper mapper) : ControllerBase
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
        var result = await mediator.Send(new GetUsersByIdQuery(id), cancellationToken);

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
    /// Add a tenant to a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">>Data for adding the tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>>HTTP status code 204 (No Content).</returns>
    [HttpPost("{id}/tenant")]
    public async Task<IActionResult> AddTenant(Guid id, [FromBody] AddTenantDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<AddTenantCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Remove a tenant from a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="idTenant">The unique identifier of the tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}/tenant/{idTenant}")]
    public async Task<IActionResult> RemoveTenant(Guid id, Guid idTenant, CancellationToken cancellationToken)
    {
        var command = new RemoveTenantCommand(id, idTenant);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Add a role to a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">>Data for adding the role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>>HTTP status code 204 (No Content).</returns>
    [HttpPost("{id}/role")]
    public async Task<IActionResult> AddRole(Guid id, [FromBody] AddRoleDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<AddRoleCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Remove a role from a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="role">The role to be removed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}/role/{role}")]
    public async Task<IActionResult> RemoveRole(Guid id, string role, CancellationToken cancellationToken)
    {
        var command = new RemoveRoleCommand(id, role);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the contact information of a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">Data for updating the contact information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPatch("{id}/contact")]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] UpdateContactDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateContactCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the job information of a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">Data for updating the job information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPatch("{id}/job")]
    public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateJobCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the profile information of a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">>Data for updating the profile information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>>HTTP status code 204 (No Content).</returns>
    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateProfileCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update the picture of a User.
    /// </summary>
    /// <param name="id">The unique identifier of the User.</param>
    /// <param name="data">Data for updating the picture.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>>HTTP status code 204 (No Content).</returns>
    [HttpPatch("{id}/picture")]
    public async Task<IActionResult> UpdatePicture(Guid id, [FromBody] UpdatePictureDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdatePictureCommand>(data), cancellationToken);

        return NoContent();
    }
}