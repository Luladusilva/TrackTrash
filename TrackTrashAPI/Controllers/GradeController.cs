using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.Grades.Commands;
using TrackTrash.Application.UseCases.Grades.Queries;
using TrackTrash.Application.UseCases.PickupServices.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GradeController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GradeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGradesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GradeDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var grade = await sender.Send(new GetGradeByIdQuery { GradeId = id }, cancellationToken);
        if (!await CanAccessServiceAsync(grade.PickupServiceId, cancellationToken)) return Forbid();
        return Ok(grade);
    }

    [HttpGet("pickup-service/{pickupServiceId:long}/user/{userId:long}")]
    public async Task<ActionResult<GradeDto>> GetByServiceAndUser(long pickupServiceId, long userId, CancellationToken cancellationToken)
    {
        if (!await CanAccessServiceAsync(pickupServiceId, cancellationToken)) return Forbid();
        return Ok(await sender.Send(new GetGradeByServiceAndUserQuery { PickupServiceId = pickupServiceId, UserId = userId }, cancellationToken));
    }

    [Authorize(Roles = "Generator,Organization")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateGradeCommand command, CancellationToken cancellationToken)
    {
        command.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateGradeCommand command, CancellationToken cancellationToken)
    {
        command.GradeId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteGradeCommand { GradeId = id }, cancellationToken);
        return NoContent();
    }

    private async Task<bool> CanAccessServiceAsync(long serviceId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin")) return true;
        var service = await sender.Send(new GetPickupServiceByIdQuery { PickupServiceId = serviceId }, cancellationToken);
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = service.CollectRequestId }, cancellationToken);
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (request.UserId == userId) return true;
        if (!User.IsInRole("Gatherer")) return false;
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        return service.GathererId == gatherer.GathererId;
    }
}
