using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.PickupServices.Queries;
using TrackTrash.Application.UseCases.RequestHistories.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RequestHistoryController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestHistoryDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestHistoriesQuery(), cancellationToken));

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<RequestHistoryDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var history = await sender.Send(new GetRequestHistoryByIdQuery { RequestHistoryId = id }, cancellationToken);
        if (!await CanViewRequestAsync(history.CollectRequestId, cancellationToken)) return Forbid();
        return Ok(history);
    }

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("collect-request/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<RequestHistoryDto>>> GetByCollectRequest(long value, CancellationToken cancellationToken)
    {
        if (!await CanViewRequestAsync(value, cancellationToken)) return Forbid();
        return Ok(await sender.Send(new GetRequestHistoriesByCollectRequestQuery { CollectRequestId = value }, cancellationToken));
    }

    private async Task<bool> CanViewRequestAsync(long requestId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin")) return true;
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = requestId }, cancellationToken);
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (request.UserId == userId) return true;
        if (!User.IsInRole("Gatherer")) return false;
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        if (!gatherer.IsActive || gatherer.VerificationStatusId != TrackTrash.Domain.Enums.VerificationStatus.Verified) return false;
        if (request.StatusId == TrackTrash.Domain.Enums.CollectRequestStatus.Pending)
        {
            var compatible = await sender.Send(new GetPendingCollectRequestsQuery(userId), cancellationToken);
            return compatible.Any(x => x.CollectRequestId == requestId);
        }
        var service = await sender.Send(new GetPickupServiceByCollectRequestQuery { CollectRequestId = requestId }, cancellationToken);
        return service.GathererId == gatherer.GathererId;
    }
}
