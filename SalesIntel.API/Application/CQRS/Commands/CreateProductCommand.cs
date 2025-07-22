using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Commands;

public class CreateProductCommand : IRequest<ProductDto>
{
    public CreateProductDto Product { get; set; } = null!;
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(SalesIntelDbContext context, ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductDto> HandleAsync(CreateProductCommand request)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Product.Name);

        // Validate SKU uniqueness
        var existingSku = await _context.Products.AnyAsync(p => p.SKU == request.Product.Sku);
        if (existingSku)
        {
            throw new InvalidOperationException($"Product with SKU '{request.Product.Sku}' already exists");
        }

        var product = ProductTransformer.ToEntity(request.Product);
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created successfully: {ProductId}", product.Id);
        
        return ProductTransformer.ToDto(product);
    }
}
