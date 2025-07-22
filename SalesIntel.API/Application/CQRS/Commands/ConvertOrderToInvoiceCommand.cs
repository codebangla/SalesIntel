using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Commands;

public class ConvertOrderToInvoiceCommand : IRequest<OrderDto?>
{
    public int OrderId { get; set; }
}

public class ConvertOrderToInvoiceCommandHandler : IRequestHandler<ConvertOrderToInvoiceCommand, OrderDto?>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<ConvertOrderToInvoiceCommandHandler> _logger;

    public ConvertOrderToInvoiceCommandHandler(SalesIntelDbContext context, ILogger<ConvertOrderToInvoiceCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrderDto?> HandleAsync(ConvertOrderToInvoiceCommand request)
    {
        _logger.LogInformation("Converting order to invoice: {OrderId}", request.OrderId);

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId);

        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", request.OrderId);
            return null;
        }

        if (order.OrderType != "Order")
        {
            throw new InvalidOperationException("Only orders can be converted to invoices");
        }

        order.OrderType = "Invoice";
        order.Status = "Completed";
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Order converted to invoice successfully: {OrderId}", request.OrderId);
        
        return OrderTransformer.ToDto(order);
    }
}
