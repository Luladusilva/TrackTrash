using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.RequestDetails.Commands;
using TrackTrash.Application.UseCases.RequestDetails.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RequestDetailController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestDetailDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestDetailsQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RequestDetailDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestDetailByIdQuery { RequestDetailId = id }, cancellationToken));

    [HttpGet("collect-request/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<RequestDetailDto>>> GetByCollectRequest(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestDetailsByCollectRequestQuery { CollectRequestId = value }, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(CreateRequestDetailCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<long>> Update(long id, UpdateRequestDetailCommand command, CancellationToken cancellationToken)
    {
        command.RequestDetailId = id;
        return Ok(await sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRequestDetailCommand { RequestDetailId = id }, cancellationToken);
        return NoContent();
    }
}
