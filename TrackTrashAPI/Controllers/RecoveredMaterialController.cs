using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.PickupServices.Queries;
using TrackTrash.Application.UseCases.RecoveredMaterials.Commands;
using TrackTrash.Application.UseCases.RecoveredMaterials.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RecoveredMaterialController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecoveredMaterialDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRecoveredMaterialsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RecoveredMaterialDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var material = await sender.Send(new GetRecoveredMaterialByIdQuery { RecoveredMaterialId = id }, cancellationToken);
        if (!await CanAccessServiceAsync(material.PickupServiceId, cancellationToken)) return Forbid();
        return Ok(material);
    }

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("pickup-service/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<RecoveredMaterialDto>>> GetByPickupService(long value, CancellationToken cancellationToken)
    {
        if (!await CanAccessServiceAsync(value, cancellationToken)) return Forbid();
        return Ok(await sender.Send(new GetRecoveredMaterialsByPickupServiceQuery { PickupServiceId = value }, cancellationToken));
    }

    [Authorize(Roles = "Admin,Gatherer")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateRecoveredMaterialCommand command, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin") && !await IsAssignedGathererAsync(command.PickupServiceId, cancellationToken)) return Forbid();
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateRecoveredMaterialCommand command, CancellationToken cancellationToken)
    {
        command.RecoveredMaterialId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRecoveredMaterialCommand { RecoveredMaterialId = id }, cancellationToken);
        return NoContent();
    }

    private async Task<bool> CanAccessServiceAsync(long serviceId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin")) return true;
        var service = await sender.Send(new GetPickupServiceByIdQuery { PickupServiceId = serviceId }, cancellationToken);
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = service.CollectRequestId }, cancellationToken);
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (request.UserId == userId) return true;
        return User.IsInRole("Gatherer") && await IsAssignedGathererAsync(serviceId, cancellationToken);
    }

    private async Task<bool> IsAssignedGathererAsync(long serviceId, CancellationToken cancellationToken)
    {
        var service = await sender.Send(new GetPickupServiceByIdQuery { PickupServiceId = serviceId }, cancellationToken);
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        return service.GathererId == gatherer.GathererId;
    }
}
