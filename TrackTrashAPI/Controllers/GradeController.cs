using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Grades.Commands;
using TrackTrash.Application.UseCases.Grades.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GradeController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GradeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGradesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GradeDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGradeByIdQuery { GradeId = id }, cancellationToken));

    [HttpGet("pickup-service/{pickupServiceId:long}/user/{userId:long}")]
    public async Task<ActionResult<GradeDto>> GetByServiceAndUser(long pickupServiceId, long userId, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGradeByServiceAndUserQuery { PickupServiceId = pickupServiceId, UserId = userId }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateGradeCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateGradeCommand command, CancellationToken cancellationToken)
    {
        command.GradeId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteGradeCommand { GradeId = id }, cancellationToken);
        return NoContent();
    }
}
