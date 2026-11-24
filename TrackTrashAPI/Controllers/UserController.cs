using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Application.UseCases.Users.Commands;
using TrackTrash.Application.UseCases.Users.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController(ISender sender, IAuthService authService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUsersQuery(), cancellationToken));

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return Ok(await sender.Send(new GetUserByIdQuery { UserId = userId }, cancellationToken));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserDto>> GetById(long id, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin") && id != GetCurrentUserId()) return Forbid();
        return Ok(await sender.Send(new GetUserByIdQuery { UserId = id }, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("email/{value}")]
    public async Task<ActionResult<UserDto>> GetByEmail(string value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetUserByEmailQuery { Email = value }, cancellationToken));

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (command.RoleId == TrackTrash.Domain.Enums.UserRole.Admin && !User.IsInRole("Admin"))
            return BadRequest(new { error = "The Admin role can only be assigned by an administrator." });

        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin") && id != GetCurrentUserId()) return Forbid();
        var current = await sender.Send(new GetUserByIdQuery { UserId = id }, cancellationToken);
        if (!User.IsInRole("Admin"))
        {
            command.RoleId = current.RoleId;
            command.IsActive = current.IsActive;
        }
        command.UserId = id;
        var updatedId = await sender.Send(command, cancellationToken);
        if (current.RoleId != command.RoleId || current.IsActive != command.IsActive)
            await authService.RevokeUserSessionsAsync(id, cancellationToken);
        return Ok(updatedId);
    }

    [HttpPut("{id:long}/password")]
    public async Task<ActionResult<bool>> ChangePassword(long id, ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin") && id != GetCurrentUserId()) return Forbid();
        command.UserId = id;
        var changed = await sender.Send(command, cancellationToken);
        await authService.RevokeUserSessionsAsync(id, cancellationToken);
        return Ok(changed);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        // Soft delete: the user is deactivated so their history is kept.
        await sender.Send(new DeleteUserCommand { UserId = id }, cancellationToken);
        await authService.RevokeUserSessionsAsync(id, cancellationToken);
        return NoContent();
    }

    private long GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("The authenticated token does not contain a valid user identifier.");
    }
}
