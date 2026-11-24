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
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetOrganizationsQuery(), cancellationToken));

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrganizationDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var organization = await sender.Send(new GetOrganizationByIdQuery { OrganizationId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && !User.IsInRole("Gatherer") &&
            organization.UserId != long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        return Ok(User.IsInRole("Admin") ? organization : organization with { TaxId = null });
    }

    [Authorize(Roles = "Admin,Organization,Gatherer")]
    [HttpGet("user/{value:long}")]
    public async Task<ActionResult<OrganizationDto>> GetByUser(long value, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!User.IsInRole("Admin") && !User.IsInRole("Gatherer") && value != userId) return Forbid();
        var organization = await sender.Send(new GetOrganizationByUserQuery { UserId = value }, cancellationToken);
        return Ok(User.IsInRole("Admin") ? organization : organization with { TaxId = null });
    }

    [Authorize(Roles = "Admin,Organization")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Organization")) command.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin,Organization")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = await sender.Send(new GetOrganizationByIdQuery { OrganizationId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && organization.UserId != long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        if (!User.IsInRole("Admin"))
        {
            command.IsActive = organization.IsActive;
            command.SubscriptionPlanId = organization.SubscriptionPlanId;
        }
        command.OrganizationId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteOrganizationCommand { OrganizationId = id }, cancellationToken);
        return NoContent();
    }
}
