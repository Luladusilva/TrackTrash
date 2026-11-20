using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Auth.Commands;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto login)
    {
        var result = await sender.Send(new LoginCommand(login));

        if (result is null)
            return Unauthorized();

        return Ok(result);
    }
}
