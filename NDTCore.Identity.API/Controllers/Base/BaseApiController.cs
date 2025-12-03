using Microsoft.AspNetCore.Mvc;

namespace NDTCore.Identity.API.Controllers.Base;

/// <summary>
/// Base controller for all API controllers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public abstract class BaseApiController : ControllerBase
{
}