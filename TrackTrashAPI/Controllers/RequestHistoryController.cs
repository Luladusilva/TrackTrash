using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.RequestHistories.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RequestHistoryController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestHistoryDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestHistoriesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RequestHistoryDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestHistoryByIdQuery { RequestHistoryId = id }, cancellationToken));

    [HttpGet("collect-request/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<RequestHistoryDto>>> GetByCollectRequest(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetRequestHistoriesByCollectRequestQuery { CollectRequestId = value }, cancellationToken));
}
