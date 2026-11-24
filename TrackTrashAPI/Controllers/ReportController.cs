using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Ports.Services;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Users.Queries;

namespace TrackTrashAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]/[action]")]
public class ReportController(ISender sender, IXMLService xmlService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await sender.Send(new GetUsersQuery());
        var file = xmlService.CreateUsersReport(users);

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "usuarios.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> CollectRequests()
    {
        var requests = await sender.Send(new GetCollectRequestsQuery());
        var file = xmlService.CreateCollectRequestsReport(requests);

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "solicitudes.xlsx");
    }
}
