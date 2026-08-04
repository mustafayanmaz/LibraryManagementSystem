using LibraryManagement.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public HealthController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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

    [HttpGet("database")]
    public async Task<IActionResult> GetDatabaseHealthStatus()
    {
        var canConnect = await _dbContext.Database.CanConnectAsync();

        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "error",
                message = "PostgreSQL veritabanına bağlanılamadı."
            });
        }

        return Ok(new
        {
            status = "success",
            message = "PostgreSQL veritabanı bağlantısı başarılı."
        });
    }
}