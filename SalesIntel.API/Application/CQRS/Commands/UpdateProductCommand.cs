using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Commands;

public class UpdateProductCommand : IRequest<ProductDto?>
{
    public int Id { get; set; }
    public UpdateProductDto Product { get; set; } = null!;
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(SalesIntelDbContext context, ILogger<UpdateProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductDto?> HandleAsync(UpdateProductCommand request)
    {
        _logger.LogInformation("Updating product: {ProductId}", request.Id);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id);
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", request.Id);
            return null;
        }

        // Validate SKU uniqueness (excluding current product)
        var existingSku = await _context.Products
            .AnyAsync(p => p.SKU == request.Product.Sku && p.Id != request.Id);
        if (existingSku)
        {
            throw new InvalidOperationException($"Product with SKU '{request.Product.Sku}' already exists");
        }

        ProductTransformer.UpdateEntity(product, request.Product);
        
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product updated successfully: {ProductId}", product.Id);
        
        return ProductTransformer.ToDto(product);
    }
}
