using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.GathererGrades.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GathererGradeController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GathererGradeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererGradesQuery(), cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<GathererGradeDto>> GetById(long id, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererGradeByIdQuery { GathererGradeId = id }, cancellationToken));

    [HttpGet("gatherer/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<GathererGradeDto>>> GetByGatherer(long value, CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererGradesByGathererQuery { GathererId = value }, cancellationToken));
}
