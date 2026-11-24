using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Gatherers.Commands;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.CollectRequests.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GathererController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GathererDto>>> GetAll(CancellationToken cancellationToken)
    {
        var gatherers = await sender.Send(new GetGatherersQuery(), cancellationToken);
        return Ok(User.IsInRole("Admin") ? gatherers : gatherers.Select(RedactVerificationDocument).ToList());
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<GathererDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var gatherer = await sender.Send(new GetGathererByIdQuery { GathererId = id }, cancellationToken);
        return Ok(User.IsInRole("Admin") ? gatherer : RedactVerificationDocument(gatherer));
    }

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("user/{value:long}")]
    public async Task<ActionResult<GathererDto>> GetByUser(long value, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Gatherer") && value != long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)) return Forbid();
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = value }, cancellationToken);
        return Ok(User.IsInRole("Admin") ? gatherer : RedactVerificationDocument(gatherer));
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpGet("available/residue-type/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<GathererDto>>> GetAvailableByResidueType(long value, CancellationToken cancellationToken)
    {
        var gatherers = await sender.Send(new GetAvailableGatherersByResidueTypeQuery { ResidueTypeId = value }, cancellationToken);
        return Ok(gatherers.Select(RedactVerificationDocument).ToList());
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpGet("available/collect-request/{collectRequestId:long}")]
    public async Task<ActionResult<IReadOnlyList<GathererDto>>> GetAvailableForCollectRequest(long collectRequestId, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin"))
        {
            var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = collectRequestId }, cancellationToken);
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (request.UserId != userId) return Forbid();
        }
        var gatherers = await sender.Send(new GetAvailableGatherersForCollectRequestQuery(collectRequestId), cancellationToken);
        return Ok(gatherers.Select(RedactVerificationDocument).ToList());
    }

    [Authorize(Roles = "Gatherer")]
    [HttpGet("me/applications")]
    public async Task<ActionResult<IReadOnlyList<CollectRequestApplicationDto>>> GetMyApplications(CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await sender.Send(new GetMyCollectRequestApplicationsQuery(userId), cancellationToken));
    }

    [Authorize(Roles = "Admin,Gatherer")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateGathererCommand command, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Gatherer")) command.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin,Gatherer")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateGathererCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var gatherer = await sender.Send(new GetGathererByIdQuery { GathererId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && gatherer.UserId != currentUserId) return Forbid();
        if (!User.IsInRole("Admin"))
        {
            command.VerificationStatusId = gatherer.VerificationStatusId;
            command.IsActive = gatherer.IsActive;
        }
        command.GathererId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Gatherer")]
    [HttpPut("me/residue-types")]
    public async Task<IActionResult> SetMyResidueTypes(SetGathererResidueTypesDto request, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        await sender.Send(new SetGathererResidueTypesCommand(
            gatherer.GathererId, userId, false, request.ResidueTypeIds), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:long}/residue-types")]
    public async Task<IActionResult> SetResidueTypes(long id, SetGathererResidueTypesDto request, CancellationToken cancellationToken)
    {
        var actorUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await sender.Send(new SetGathererResidueTypesCommand(id, actorUserId, true, request.ResidueTypeIds), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteGathererCommand { GathererId = id }, cancellationToken);
        return NoContent();
    }

    private static GathererDto RedactVerificationDocument(GathererDto gatherer) =>
        gatherer with { VerificationDocument = null };
}
