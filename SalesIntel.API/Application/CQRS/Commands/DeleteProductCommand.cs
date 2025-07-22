using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Commands;

public class DeleteProductCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(SalesIntelDbContext context, ILogger<DeleteProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteProductCommand request)
    {
        _logger.LogInformation("Deleting product: {ProductId}", request.Id);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id);
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", request.Id);
            return;
        }

        // Check if product is used in orders
        var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == request.Id);
        if (hasOrders)
        {
            throw new InvalidOperationException("Cannot delete product that is referenced in orders");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product deleted successfully: {ProductId}", request.Id);
    }
}
