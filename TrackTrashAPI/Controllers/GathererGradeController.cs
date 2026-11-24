using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Gatherers.Queries;
using TrackTrash.Application.UseCases.GathererGrades.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GathererGradeController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GathererGradeDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererGradesQuery(), cancellationToken));

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<GathererGradeDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var grade = await sender.Send(new GetGathererGradeByIdQuery { GathererGradeId = id }, cancellationToken);
        if (User.IsInRole("Gatherer"))
        {
            var current = await sender.Send(new GetGathererByUserQuery
            {
                UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            }, cancellationToken);
            if (grade.GathererId != current.GathererId) return Forbid();
        }
        return Ok(grade);
    }

    [Authorize(Roles = "Admin,Generator,Organization,Gatherer")]
    [HttpGet("gatherer/{value:long}")]
    public async Task<ActionResult<IReadOnlyList<GathererGradeDto>>> GetByGatherer(long value, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Gatherer"))
        {
            var current = await sender.Send(new GetGathererByUserQuery
            {
                UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            }, cancellationToken);
            if (value != current.GathererId) return Forbid();
        }
        return Ok(await sender.Send(new GetGathererGradesByGathererQuery { GathererId = value }, cancellationToken));
    }
}
