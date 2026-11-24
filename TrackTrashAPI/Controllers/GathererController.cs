using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Gatherers.Commands;
using TrackTrash.Application.UseCases.Gatherers.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GathererController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GathererDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGatherersQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GathererDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererByIdQuery { GathererId = id }, cancellationToken));

    [HttpGet("user/{value:long}")]
    public async Task<ActionResult<GathererDto>> GetByUser(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererByUserQuery { UserId = value }, cancellationToken));

    [HttpGet("available/residue-type/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<GathererDto>>> GetAvailableByResidueType(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetAvailableGatherersByResidueTypeQuery { ResidueTypeId = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateGathererCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateGathererCommand command, CancellationToken cancellationToken)
    {
        command.GathererId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteGathererCommand { GathererId = id }, cancellationToken);
        return NoContent();
    }
}
