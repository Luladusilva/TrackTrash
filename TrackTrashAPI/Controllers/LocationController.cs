using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Locations.Commands;
using TrackTrash.Application.UseCases.Locations.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LocationController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocationDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetLocationsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<LocationDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetLocationByIdQuery { LocationId = id }, cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin,Generator,Organization")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        command.LocationId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteLocationCommand { LocationId = id }, cancellationToken);
        return NoContent();
    }
}
