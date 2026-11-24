using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.CollectRequests.Commands;
using TrackTrash.Application.UseCases.CollectRequests.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CollectRequestController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CollectRequestDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectRequestsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CollectRequestDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectRequestByIdQuery { CollectRequestId = id }, cancellationToken));

    [HttpGet("user/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<CollectRequestDto>>> GetByUser(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectRequestsByUserQuery { UserId = value }, cancellationToken));

    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<CollectRequestDto>>> GetPending(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetPendingCollectRequestsQuery(), cancellationToken));

    [HttpGet("{id:long}/full")]
    public async Task<ActionResult<CollectRequestDetailDto>> GetWithDetails(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetCollectRequestWithDetailsQuery { CollectRequestId = id }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateCollectRequestCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateCollectRequestCommand command, CancellationToken cancellationToken)
    {
        command.CollectRequestId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCollectRequestCommand { CollectRequestId = id }, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<long>> ChangeStatus(long id, ChangeCollectRequestStatusCommand command, CancellationToken cancellationToken)
    {
        command.CollectRequestId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }
}
