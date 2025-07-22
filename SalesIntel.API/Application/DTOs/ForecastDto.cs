namespace SalesIntel.API.Application.DTOs;

public class ForecastDto
{
    public DateTime Date { get; set; }
    public decimal MovingAverage { get; set; }
    public decimal LinearTrend { get; set; }
    public decimal Seasonal { get; set; }
    public decimal Capacity { get; set; }
}

public class ForecastParametersDto
{
    public int ShortTermPeriod { get; set; } = 7;
    public int TrendPeriod { get; set; } = 30;
    public int SeasonalPeriod { get; set; } = 90;
    public decimal ResourceFactor { get; set; } = 1.2m;
}

public class InventoryAlertDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int FutureInvoiceQuantities { get; set; }
    public int Shortfall { get; set; }
    public string Severity { get; set; } = string.Empty;
    public int DaysUntilStockout { get; set; }
}
