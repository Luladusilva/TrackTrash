using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TrackTrashAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PickupServiceController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
}
