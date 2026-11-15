using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndicatorController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
}
