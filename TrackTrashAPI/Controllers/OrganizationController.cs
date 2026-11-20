using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Organizations.Commands;
using TrackTrash.Application.UseCases.Organizations.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrganizationController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetOrganizationsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrganizationDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetOrganizationByIdQuery { OrganizationId = id }, cancellationToken));

    [HttpGet("user/{value:long}")]
    public async Task<ActionResult<OrganizationDto>> GetByUser(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetOrganizationByUserQuery { UserId = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        command.OrganizationId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteOrganizationCommand { OrganizationId = id }, cancellationToken);
        return NoContent();
    }
}
