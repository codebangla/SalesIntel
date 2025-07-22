using Microsoft.AspNetCore.Mvc;
using SalesIntel.API.Application.Services;
using SalesIntel.API.Application.DTOs;

namespace SalesIntel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForecastingController : ControllerBase
{
    private readonly IForecastingService _forecastingService;
    private readonly ILogger<ForecastingController> _logger;

    public ForecastingController(IForecastingService forecastingService, ILogger<ForecastingController> logger)
    {
        _forecastingService = forecastingService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<List<ForecastDto>>> GenerateForecast([FromBody] ForecastParametersDto parameters)
    {
        var forecast = await _forecastingService.GenerateForecastAsync(parameters);
        return Ok(forecast);
    }

    [HttpGet("inventory-alerts")]
    public async Task<ActionResult<List<InventoryAlertDto>>> GetInventoryAlerts()
    {
        var alerts = await _forecastingService.GetInventoryAlertsAsync();
        return Ok(alerts);
    }

    [HttpGet("moving-average/{productId}")]
    public async Task<ActionResult<decimal>> GetMovingAverage(int productId, [FromQuery] int days = 7)
    {
        var result = await _forecastingService.CalculateMovingAverageAsync(productId, days);
        return Ok(result);
    }
}
