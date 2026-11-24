using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackTrash.Application.Dtos;
using TrackTrash.Application.UseCases.Indicators.Queries;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class IndicatorController(ISender sender) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("general")]
    public async Task<ActionResult<GeneralIndicatorDto>> GetGeneral(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGeneralIndicatorsQuery(), cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpGet("residues-by-type")]
    public async Task<ActionResult<IReadOnlyList<ResiduesByTypeDto>>> GetResiduesByType(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetResiduesByTypeQuery(), cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpGet("gatherer-ratings")]
    public async Task<ActionResult<IReadOnlyList<GathererRatingDto>>> GetGathererRatings(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetGathererRatingsQuery(), cancellationToken));
}
