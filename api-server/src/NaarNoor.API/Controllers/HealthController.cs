using Microsoft.AspNetCore.Mvc;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("/api/healthz")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });
}
