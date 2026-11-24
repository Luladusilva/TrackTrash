using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.ResidueTypes.Commands;
using TrackTrash.Application.UseCases.ResidueTypes.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ResidueTypeController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ResidueTypeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetResidueTypesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ResidueTypeDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetResidueTypeByIdQuery { ResidueTypeId = id }, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateResidueTypeCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateResidueTypeCommand command, CancellationToken cancellationToken)
    {
        command.ResidueTypeId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteResidueTypeCommand { ResidueTypeId = id }, cancellationToken);
        return NoContent();
    }
}
