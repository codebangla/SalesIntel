using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Queries;

public class GetProductsQuery : IRequest<List<ProductDto>>
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(SalesIntelDbContext context, ILogger<GetProductsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ProductDto>> HandleAsync(GetProductsQuery request)
    {
        _logger.LogInformation("Getting products with search: {Search}", request.Search);

        var query = _context.Products.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p => p.ProductName.Contains(request.Search) || 
                                   p.SKU.Contains(request.Search) ||
                                   p.Category.Contains(request.Search));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
                "price" => request.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "stock" => request.SortDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
                _ => query.OrderBy(p => p.Id)
            };
        }

        // Apply pagination
        var products = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return ProductTransformer.ToDtoList(products);
    }
}
