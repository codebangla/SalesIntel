using Microsoft.AspNetCore.Mvc;
using SalesIntel.API.Application.CQRS;
using SalesIntel.API.Application.CQRS.Queries;
using SalesIntel.API.Application.CQRS.Commands;
using SalesIntel.API.Application.DTOs;

namespace SalesIntel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? type,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOrdersQuery
        {
            Search = search,
            Type = type,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Page = page,
            PageSize = pageSize
        };

        var orders = await _mediator.SendAsync(query);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto orderDto)
    {
        var command = new CreateOrderCommand { Order = orderDto };
        var order = await _mediator.SendAsync(command);
        
        return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
    }

    [HttpPost("{id}/convert-to-invoice")]
    public async Task<ActionResult<OrderDto>> ConvertToInvoice(int id)
    {
        var command = new ConvertOrderToInvoiceCommand { OrderId = id };
        var order = await _mediator.SendAsync(command);
        
        if (order == null)
            return NotFound();

        return Ok(order);
    }
}
