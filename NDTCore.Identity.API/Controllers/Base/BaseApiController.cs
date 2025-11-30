using Microsoft.AspNetCore.Mvc;

namespace NDTCore.Identity.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public abstract class BaseApiController : ControllerBase
{
}