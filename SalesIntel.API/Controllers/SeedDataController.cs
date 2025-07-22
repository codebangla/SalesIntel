using Microsoft.AspNetCore.Mvc;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedDataController : ControllerBase
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<SeedDataController> _logger;

    public SeedDataController(SalesIntelDbContext context, ILogger<SeedDataController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("reload")]
    public async Task<ActionResult> ReloadSeedData()
    {
        _logger.LogInformation("Reloading seed data");

        try
        {
            // Clear existing data
            _context.OrderItems.RemoveRange(_context.OrderItems);
            _context.Orders.RemoveRange(_context.Orders);
            _context.Inventory.RemoveRange(_context.Inventory);
            _context.Products.RemoveRange(_context.Products);
            
            await _context.SaveChangesAsync();

            // Reload seed data
            await SeedData.SeedAsync(_context);

            _logger.LogInformation("Seed data reloaded successfully");
            return Ok(new { message = "Seed data reloaded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading seed data");
            return StatusCode(500, new { message = "Error reloading seed data", error = ex.Message });
        }
    }
}
