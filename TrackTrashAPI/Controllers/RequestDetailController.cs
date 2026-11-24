using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.PickupServices.Queries;
using TrackTrash.Application.UseCases.RequestDetails.Commands;
using TrackTrash.Application.UseCases.RequestDetails.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RequestDetailController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestDetailDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestDetailsQuery(), cancellationToken));

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<RequestDetailDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var detail = await sender.Send(new GetRequestDetailByIdQuery { RequestDetailId = id }, cancellationToken);
        if (!await CanViewRequestAsync(detail.CollectRequestId, cancellationToken)) return Forbid();
        return Ok(detail);
    }

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("collect-request/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<RequestDetailDto>>> GetByCollectRequest(long value, CancellationToken cancellationToken)
    {
        if (!await CanViewRequestAsync(value, cancellationToken)) return Forbid();
        return Ok(await sender.Send(new GetRequestDetailsByCollectRequestQuery { CollectRequestId = value }, cancellationToken));
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateRequestDetailCommand command, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin") && !await IsOwnerAsync(command.CollectRequestId, cancellationToken)) return Forbid();
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateRequestDetailCommand command, CancellationToken cancellationToken)
    {
        var detail = await sender.Send(new GetRequestDetailByIdQuery { RequestDetailId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && !await IsOwnerAsync(detail.CollectRequestId, cancellationToken)) return Forbid();
        command.RequestDetailId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRequestDetailCommand { RequestDetailId = id }, cancellationToken);
        return NoContent();
    }

    private async Task<bool> IsOwnerAsync(long requestId, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = requestId }, cancellationToken);
        return request.UserId == long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private async Task<bool> CanViewRequestAsync(long requestId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin") || await IsOwnerAsync(requestId, cancellationToken)) return true;
        if (!User.IsInRole("Gatherer")) return false;
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        if (!gatherer.IsActive || gatherer.VerificationStatusId != TrackTrash.Domain.Enums.VerificationStatus.Verified) return false;
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = requestId }, cancellationToken);
        if (request.StatusId == TrackTrash.Domain.Enums.CollectRequestStatus.Pending)
        {
            var compatible = await sender.Send(new GetPendingCollectRequestsQuery(userId), cancellationToken);
            return compatible.Any(x => x.CollectRequestId == requestId);
        }
        var service = await sender.Send(new GetPickupServiceByCollectRequestQuery { CollectRequestId = requestId }, cancellationToken);
        return service.GathererId == gatherer.GathererId;
    }
}
