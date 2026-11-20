using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectionPoints.Commands;
using TrackTrash.Application.UseCases.CollectionPoints.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CollectionPointController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CollectionPointDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectionPointsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CollectionPointDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectionPointByIdQuery { CollectionPointId = id }, cancellationToken));

    [HttpGet("residue-type/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<CollectionPointDto>>> GetActiveByResidueType(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetActiveCollectionPointsByResidueTypeQuery { ResidueTypeId = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateCollectionPointCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateCollectionPointCommand command, CancellationToken cancellationToken)
    {
        command.CollectionPointId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCollectionPointCommand { CollectionPointId = id }, cancellationToken);
        return NoContent();
    }
}
