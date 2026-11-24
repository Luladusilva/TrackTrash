using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.RecoveredMaterials.Commands;
using TrackTrash.Application.UseCases.RecoveredMaterials.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RecoveredMaterialController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecoveredMaterialDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRecoveredMaterialsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RecoveredMaterialDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRecoveredMaterialByIdQuery { RecoveredMaterialId = id }, cancellationToken));

    [HttpGet("pickup-service/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<RecoveredMaterialDto>>> GetByPickupService(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRecoveredMaterialsByPickupServiceQuery { PickupServiceId = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateRecoveredMaterialCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateRecoveredMaterialCommand command, CancellationToken cancellationToken)
    {
        command.RecoveredMaterialId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRecoveredMaterialCommand { RecoveredMaterialId = id }, cancellationToken);
        return NoContent();
    }
}
