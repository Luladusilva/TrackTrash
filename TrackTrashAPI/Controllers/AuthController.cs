using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Application.UseCases.Auth.Commands;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(ISender sender, IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registration, CancellationToken cancellationToken)
    {
        var id = await sender.Send(new RegisterCommand(registration), cancellationToken);
        return CreatedAtAction(nameof(UserController.GetById), "User", new { id }, id);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto login, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginCommand(login), cancellationToken);

        if (result is null)
            return Unauthorized();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Refresh(RefreshTokenDto request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request.RefreshToken, cancellationToken);
        return result is null
            ? Unauthorized(new { code = "INVALID_REFRESH_TOKEN", error = "The refresh token is invalid or expired." })
            : Ok(result);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Logout(RefreshTokenDto request, CancellationToken cancellationToken)
    {
        await authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }
}
