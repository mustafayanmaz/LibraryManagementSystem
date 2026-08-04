using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealthStatus()
    {
        return Ok(new
        {
            status = "success",
            message = "Library Management API çalışıyor.",
            timestamp = DateTime.UtcNow
        });
    }
}