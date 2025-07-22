using SalesIntel.API.Application.DTOs;

namespace SalesIntel.API.Application.Services;

public interface IForecastingService
{
    Task<List<ForecastDto>> GenerateForecastAsync(ForecastParametersDto parameters);
    Task<List<InventoryAlertDto>> GetInventoryAlertsAsync();
    Task<decimal> CalculateMovingAverageAsync(int productId, int days);
    Task<decimal> CalculateLinearTrendAsync(int productId, int days);
    Task<decimal> CalculateSeasonalForecastAsync(int productId, int days);
}
