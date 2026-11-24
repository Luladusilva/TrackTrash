using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Users.Commands;
using TrackTrash.Application.UseCases.Users.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUsersQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUserByIdQuery { UserId = id }, cancellationToken));

    [HttpGet("email/{value}")]
    public async Task<ActionResult<UserDto>> GetByEmail(string value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUserByEmailQuery { Email = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        command.UserId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpPut("{id:long}/password")]
    public async Task<ActionResult<bool>> ChangePassword(long id, ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        command.UserId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        // Soft delete: the user is deactivated so their history is kept.
        await sender.Send(new DeleteUserCommand { UserId = id }, cancellationToken);
        return NoContent();
    }
}
