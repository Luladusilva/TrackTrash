using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Commands;
using TrackTrash.Application.UseCases.CollectRequests.Queries;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.PickupServices.Queries;
using TrackTrash.Application.UseCases.Users.Queries;
using TrackTrash.Application.UseCases.Organizations.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CollectRequestController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CollectRequestDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectRequestsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CollectRequestDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
        if (!await CanViewRequestAsync(request, cancellationToken)) return Forbid();
        return Ok(request);
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpGet("user/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<CollectRequestDto>>> GetByUser(long value, CancellationToken cancellationToken)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!User.IsInRole("Admin") && value != currentUserId) return Forbid();
        return Ok(await sender.Send(new GetCollectRequestsByUserQuery { UserId = value }, cancellationToken));
    }

    [Authorize(Roles = "Admin,Gatherer")]
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<CollectRequestDto>>> GetPending(CancellationToken cancellationToken)
    {
        long? userId = User.IsInRole("Admin") ? null : long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await sender.Send(new GetPendingCollectRequestsQuery(userId), cancellationToken));
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpGet("{id:long}/applications")]
    public async Task<ActionResult<IReadOnlyList<CollectRequestApplicationDto>>> GetApplications(long id, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin"))
        {
            var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
            if (!IsOwner(request)) return Forbid();
        }

        return Ok(await sender.Send(new GetCollectRequestApplicationsQuery(id), cancellationToken));
    }

    [Authorize(Roles = "Gatherer")]
    [HttpPost("{id:long}/applications")]
    public async Task<ActionResult<long>> Apply(long id, CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var applicationId = await sender.Send(new ApplyForCollectRequestCommand(id, userId), cancellationToken);
        return Ok(new { applicationId });
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPost("{id:long}/applications/{applicationId:long}/accept")]
    public async Task<ActionResult<long>> AcceptApplication(long id, long applicationId, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin"))
        {
            var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
            if (!IsOwner(request)) return Forbid();
        }
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var serviceId = await sender.Send(new AcceptCollectRequestApplicationCommand(
            id, applicationId, userId, User.IsInRole("Admin")), cancellationToken);
        return Ok(new { pickupServiceId = serviceId });
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPost("{id:long}/applications/{applicationId:long}/reject")]
    public async Task<IActionResult> RejectApplication(long id, long applicationId, CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Admin"))
        {
            var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
            if (!IsOwner(request)) return Forbid();
        }
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await sender.Send(new RejectCollectRequestApplicationCommand(
            id, applicationId, userId, User.IsInRole("Admin")), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:long}/full")]
    public async Task<ActionResult<CollectRequestDetailDto>> GetWithDetails(long id, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
        if (!await CanViewRequestAsync(request, cancellationToken)) return Forbid();
        return Ok(await sender.Send(new GetCollectRequestWithDetailsQuery { CollectRequestId = id }, cancellationToken));
    }

    [HttpGet("{id:long}/requester-profile")]
    public async Task<ActionResult<RequesterProfileDto>> GetRequesterProfile(long id, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
        if (!await CanViewRequestAsync(request, cancellationToken)) return Forbid();

        var user = await sender.Send(new GetUserByIdQuery { UserId = request.UserId }, cancellationToken);
        var organizationName = user.RoleId == TrackTrash.Domain.Enums.UserRole.Organization
            ? (await sender.Send(new GetOrganizationByUserQuery { UserId = request.UserId }, cancellationToken)).BusinessName
            : null;
        return Ok(new RequesterProfileDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RoleId = user.RoleId,
            LocationId = user.LocationId,
            OrganizationName = organizationName
        });
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateCollectRequestCommand command, CancellationToken cancellationToken)
    {
        command.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateCollectRequestCommand command, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && !IsOwner(request)) return Forbid();
        command.CollectRequestId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && !IsOwner(request)) return Forbid();
        await sender.Send(new DeleteCollectRequestCommand { CollectRequestId = id }, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Generator,Organization")]
    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<long>> ChangeStatus(long id, ChangeCollectRequestStatusCommand command, CancellationToken cancellationToken)
    {
        var request = await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken);
        if (!User.IsInRole("Admin") && (!IsOwner(request) || command.NewStatusId != TrackTrash.Domain.Enums.CollectRequestStatus.Cancelled))
            return Forbid();
        command.CollectRequestId = id;
        command.ChangedByUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await sender.Send(command, cancellationToken));
    }

    private bool IsOwner(CollectRequestDto request) =>
        request.UserId == long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<bool> CanViewRequestAsync(CollectRequestDto request, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Admin") || IsOwner(request)) return true;
        if (!User.IsInRole("Gatherer")) return false;

        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var gatherer = await sender.Send(new GetGathererByUserQuery { UserId = userId }, cancellationToken);
        if (gatherer.VerificationStatusId != TrackTrash.Domain.Enums.VerificationStatus.Verified || !gatherer.IsActive)
            return false;

        if (request.StatusId == TrackTrash.Domain.Enums.CollectRequestStatus.Pending)
        {
            var compatible = await sender.Send(new GetPendingCollectRequestsQuery(userId), cancellationToken);
            return compatible.Any(x => x.CollectRequestId == request.CollectRequestId);
        }

        var service = await sender.Send(new GetPickupServiceByCollectRequestQuery
        {
            CollectRequestId = request.CollectRequestId
        }, cancellationToken);
        return service.GathererId == gatherer.GathererId;
    }
}
