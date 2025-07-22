using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.Services;

public class ForecastingService : IForecastingService
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<ForecastingService> _logger;

    public ForecastingService(SalesIntelDbContext context, ILogger<ForecastingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ForecastDto>> GenerateForecastAsync(ForecastParametersDto parameters)
    {
        _logger.LogInformation("Generating forecast with parameters: {@Parameters}", parameters);

        var forecasts = new List<ForecastDto>();
        var baseDate = DateTime.UtcNow.Date;

        // Get historical sales data for calculations
        var historicalData = await GetHistoricalSalesDataAsync(90); // Last 90 days

        for (int i = 0; i < 30; i++) // 30-day forecast
        {
            var forecastDate = baseDate.AddDays(i);
            
            var movingAvg = await CalculateMovingAverageForDateAsync(historicalData, parameters.ShortTermPeriod);
            var linearTrend = await CalculateLinearTrendForDateAsync(historicalData, parameters.TrendPeriod);
            var seasonal = await CalculateSeasonalForecastForDateAsync(historicalData, parameters.SeasonalPeriod, i);
            
            var capacity = ((movingAvg + linearTrend + seasonal) / 3) * parameters.ResourceFactor;

            forecasts.Add(new ForecastDto
            {
                Date = forecastDate,
                MovingAverage = Math.Round(movingAvg, 2),
                LinearTrend = Math.Round(linearTrend, 2),
                Seasonal = Math.Round(seasonal, 2),
                Capacity = Math.Round(capacity, 2)
            });
        }

        return forecasts;
    }

    public async Task<List<InventoryAlertDto>> GetInventoryAlertsAsync()
    {
        _logger.LogInformation("Generating inventory alerts");

        var alerts = new List<InventoryAlertDto>();

        var products = await _context.Products
            .Include(p => p.InventoryRecords)
            .Include(p => p.OrderItems)
            .ThenInclude(oi => oi.Order)
            .ToListAsync();

        foreach (var product in products)
        {
            var currentStock = product.InventoryRecords.FirstOrDefault()?.CurrentStock ?? product.StockQuantity;
            
            // Calculate future invoice quantities (orders that will become invoices)
            var futureInvoiceQuantities = product.OrderItems
                .Where(oi => oi.Order.OrderType == "Order" && oi.Order.Status != "Cancelled")
                .Sum(oi => oi.Quantity);

            if (currentStock < futureInvoiceQuantities)
            {
                var shortfall = futureInvoiceQuantities - currentStock;
                var severity = GetSeverity(shortfall, currentStock);
                var daysUntilStockout = CalculateDaysUntilStockout(currentStock, futureInvoiceQuantities);

                alerts.Add(new InventoryAlertDto
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    Sku = product.SKU,
                    CurrentStock = currentStock,
                    FutureInvoiceQuantities = futureInvoiceQuantities,
                    Shortfall = shortfall,
                    Severity = severity,
                    DaysUntilStockout = daysUntilStockout
                });
            }
        }

        return alerts.OrderByDescending(a => GetSeverityWeight(a.Severity)).ToList();
    }

    public async Task<decimal> CalculateMovingAverageAsync(int productId, int days)
    {
        var salesData = await GetProductSalesDataAsync(productId, days);
        return salesData.Any() ? salesData.Average() : 0;
    }

    public async Task<decimal> CalculateLinearTrendAsync(int productId, int days)
    {
        var salesData = await GetProductSalesDataAsync(productId, days);
        if (salesData.Count < 2) return 0;

        // Simple linear regression calculation
        var n = salesData.Count;
        var sumX = Enumerable.Range(1, n).Sum();
        var sumY = salesData.Sum();
        var sumXY = salesData.Select((y, i) => (i + 1) * y).Sum();
        var sumX2 = Enumerable.Range(1, n).Select(x => x * x).Sum();

        var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        var intercept = (sumY - slope * sumX) / n;

        // Predict next value
        return slope * (n + 1) + intercept;
    }

    public async Task<decimal> CalculateSeasonalForecastAsync(int productId, int days)
    {
        var salesData = await GetProductSalesDataAsync(productId, days);
        if (salesData.Count < 7) return 0;

        // Simple seasonal calculation using rolling average
        var seasonalPeriod = Math.Min(7, salesData.Count); // Weekly seasonality
        var recentData = salesData.TakeLast(seasonalPeriod);
        return recentData.Average();
    }

    private async Task<List<decimal>> GetHistoricalSalesDataAsync(int days)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        
        var salesData = await _context.Orders
            .Where(o => o.OrderDate >= startDate && o.OrderType == "Invoice")
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => g.Sum(o => o.TotalAmount))
            .ToListAsync();

        return salesData;
    }

    private async Task<List<decimal>> GetProductSalesDataAsync(int productId, int days)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        
        var salesData = await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.ProductId == productId && 
                        oi.Order.OrderDate >= startDate && 
                        oi.Order.OrderType == "Invoice")
            .GroupBy(oi => oi.Order.OrderDate.Date)
            .Select(g => g.Sum(oi => oi.TotalPrice))
            .ToListAsync();

        return salesData;
    }

    private async Task<decimal> CalculateMovingAverageForDateAsync(List<decimal> historicalData, int period)
    {
        if (historicalData.Count < period) return historicalData.Any() ? historicalData.Average() : 1000;
        return historicalData.TakeLast(period).Average();
    }

    private async Task<decimal> CalculateLinearTrendForDateAsync(List<decimal> historicalData, int period)
    {
        if (historicalData.Count < 2) return 1000;
        
        var data = historicalData.TakeLast(period).ToList();
        var n = data.Count;
        var sumX = Enumerable.Range(1, n).Sum();
        var sumY = data.Sum();
        var sumXY = data.Select((y, i) => (i + 1) * y).Sum();
        var sumX2 = Enumerable.Range(1, n).Select(x => x * x).Sum();

        if (n * sumX2 - sumX * sumX == 0) return data.Average();

        var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        var intercept = (sumY - slope * sumX) / n;

        return slope * (n + 1) + intercept;
    }

    private async Task<decimal> CalculateSeasonalForecastForDateAsync(List<decimal> historicalData, int period, int dayOffset)
    {
        if (historicalData.Count < 7) return 1000;
        
        // Simple seasonal pattern based on day of week
        var seasonalIndex = dayOffset % 7;
        var seasonalData = historicalData.Where((value, index) => index % 7 == seasonalIndex);
        
        return seasonalData.Any() ? seasonalData.Average() : historicalData.Average();
    }

    private string GetSeverity(int shortfall, int currentStock)
    {
        var ratio = (decimal)shortfall / Math.Max(currentStock, 1);
        
        if (ratio >= 2.0m || currentStock <= 5) return "critical";
        if (ratio >= 1.0m || currentStock <= 20) return "high";
        return "medium";
    }

    private int GetSeverityWeight(string severity)
    {
        return severity switch
        {
            "critical" => 3,
            "high" => 2,
            "medium" => 1,
            _ => 0
        };
    }

    private int CalculateDaysUntilStockout(int currentStock, int futureInvoiceQuantities)
    {
        if (futureInvoiceQuantities <= 0) return int.MaxValue;
        
        // Simple calculation assuming even distribution
        var dailyConsumption = futureInvoiceQuantities / 30.0; // Assume 30-day period
        return Math.Max(1, (int)Math.Ceiling(currentStock / dailyConsumption));
    }
}
