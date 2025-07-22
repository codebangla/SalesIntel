using Microsoft.EntityFrameworkCore;
using SalesIntel.API.Application.DTOs;
using SalesIntel.API.Application.Transformers;
using SalesIntel.API.Infrastructure.Data;

namespace SalesIntel.API.Application.CQRS.Queries;

public class GetOrdersQuery : IRequest<List<OrderDto>>
{
    public string? Search { get; set; }
    public string? Type { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly SalesIntelDbContext _context;
    private readonly ILogger<GetOrdersQueryHandler> _logger;

    public GetOrdersQueryHandler(SalesIntelDbContext context, ILogger<GetOrdersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<OrderDto>> HandleAsync(GetOrdersQuery request)
    {
        _logger.LogInformation("Getting orders with search: {Search}, type: {Type}", request.Search, request.Type);

        var query = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(o => o.OrderNumber.Contains(request.Search) || 
                                   o.CustomerName.Contains(request.Search));
        }

        // Apply type filter
        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            query = query.Where(o => o.OrderType.ToLower() == request.Type.ToLower());
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "customer" => request.SortDescending ? query.OrderByDescending(o => o.CustomerName) : query.OrderBy(o => o.CustomerName),
                "amount" => request.SortDescending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
                "date" => request.SortDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                _ => query.OrderByDescending(o => o.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(o => o.Id);
        }

        // Apply pagination
        var orders = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return OrderTransformer.ToDtoList(orders);
    }
}
