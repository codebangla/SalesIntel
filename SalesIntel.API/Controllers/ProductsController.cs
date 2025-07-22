using Microsoft.AspNetCore.Mvc;
using SalesIntel.API.Application.CQRS;
using SalesIntel.API.Application.CQRS.Queries;
using SalesIntel.API.Application.CQRS.Commands;
using SalesIntel.API.Application.DTOs;

namespace SalesIntel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsQuery
        {
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Page = page,
            PageSize = pageSize
        };

        var products = await _mediator.SendAsync(query);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var product = await _mediator.SendAsync(query);
        
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto productDto)
    {
        var command = new CreateProductCommand { Product = productDto };
        var product = await _mediator.SendAsync(command);
        
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
    {
        var command = new UpdateProductCommand { Id = id, Product = productDto };
        var product = await _mediator.SendAsync(command);
        
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { Id = id };
        await _mediator.SendAsync(command);
        
        return NoContent();
    }
}
