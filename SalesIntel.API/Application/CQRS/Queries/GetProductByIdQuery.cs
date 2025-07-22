using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Queries;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public int Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(SalesIntelDbContext context, ILogger<GetProductByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductDto?> HandleAsync(GetProductByIdQuery request)
    {
        _logger.LogInformation("Getting product by ID: {ProductId}", request.Id);

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id);

        return product != null ? ProductTransformer.ToDto(product) : null;
    }
}
