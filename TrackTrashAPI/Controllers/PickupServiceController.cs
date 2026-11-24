using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.PickupServices.Commands;
using TrackTrash.Application.UseCases.PickupServices.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PickupServiceController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PickupServiceDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetPickupServicesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PickupServiceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var service = await sender.Send(new GetPickupServiceByIdQuery { PickupServiceId = id }, cancellationToken);
        if (!await CanAccessServiceAsync(service, cancellationToken)) return Forbid();
        return Ok(service);
    }

    [HttpGet("collect-request/{value:long}")]
    public async Task<ActionResult<PickupServiceDto>> GetByCollectRequest(long value, CancellationToken cancellationToken)
    {
        var service = await sender.Send(new GetPickupServiceByCollectRequestQuery { CollectRequestId = value }, cancellationToken);
        if (!await CanAccessServiceAsync(service, cancellationToken)) return Forbid();
        return Ok(service);
    }

    [Authorize(Roles = "Admin,Gatherer")]
    [HttpGet("gatherer/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<PickupServiceDto>>> GetByGatherer(long value, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin"))
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
            if (gatherer.GathererId != value) return Forbid();
        }
        return Ok(await sender.Send(new GetPickupServicesByGathererQuery { GathererId = value }, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreatePickupServiceCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdatePickupServiceCommand command, CancellationToken cancellationToken)
    {
        command.PickupServiceId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeletePickupServiceCommand { PickupServiceId = id }, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Gatherer")]
    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<long>> ChangeStatus(long id, ChangePickupServiceStatusCommand command, CancellationToken cancellationToken)
    {
        var service = await sender.Send(new GetPickupServiceByIdQuery { PickupServiceId = id }, cancellationToken);
        if (!User.IsInRole("Admin"))
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
            if (service.GathererId != gatherer.GathererId) return Forbid();
        }
        command.PickupServiceId = id;
        command.ChangedByUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await sender.Send(command, cancellationToken));
    }

    private async Task<bool> CanAccessServiceAsync(PickupServiceDto service, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin")) return true;
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = service.CollectRequestId }, cancellationToken);
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (request.UserId == userId) return true;
        if (!User.IsInRole("Gatherer")) return false;
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        return service.GathererId == gatherer.GathererId;
    }
}
