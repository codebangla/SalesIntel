using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Commands;

public class CreateOrderCommand : IRequest<OrderDto>
{
    public CreateOrderDto Order { get; set; } = null!;
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(SalesIntelDbContext context, ILogger<CreateOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrderDto> HandleAsync(CreateOrderCommand request)
    {
        _logger.LogInformation("Creating order for customer: {Customer}", request.Order.Customer);

        var order = OrderTransformer.ToEntity(request.Order);
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Load the order with related data for response
        var createdOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstAsync(o => o.Id == order.Id);

        _logger.LogInformation("Order created successfully: {OrderId}", order.Id);
        
        return OrderTransformer.ToDto(createdOrder);
    }
}
