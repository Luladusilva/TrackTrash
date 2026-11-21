using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.PickupServices.Commands;
using TrackTrash.Application.UseCases.PickupServices.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PickupServiceController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PickupServiceDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetPickupServicesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PickupServiceDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetPickupServiceByIdQuery { PickupServiceId = id }, cancellationToken));

    [HttpGet("collect-request/{value:long}")]
    public async Task<ActionResult<PickupServiceDto>> GetByCollectRequest(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetPickupServiceByCollectRequestQuery { CollectRequestId = value }, cancellationToken));

    [HttpGet("gatherer/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<PickupServiceDto>>> GetByGatherer(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetPickupServicesByGathererQuery { GathererId = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreatePickupServiceCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdatePickupServiceCommand command, CancellationToken cancellationToken)
    {
        command.PickupServiceId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeletePickupServiceCommand { PickupServiceId = id }, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<long>> ChangeStatus(long id, ChangePickupServiceStatusCommand command, CancellationToken cancellationToken)
    {
        command.PickupServiceId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }
}
