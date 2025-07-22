using SalesIntel.API.Domain.Entities;
using SalesIntel.API.Application.DTOs;

namespace SalesIntel.API.Application.Transformers;

public static class ProductTransformer
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.ProductName,
            Sku = product.SKU,
            Category = product.Category,
            Price = product.Price,
            Stock = product.StockQuantity,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static Product ToEntity(CreateProductDto dto)
    {
        return new Product
        {
            ProductName = dto.Name,
            SKU = dto.Sku,
            Category = dto.Category,
            Price = dto.Price,
            StockQuantity = dto.Stock,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(Product product, UpdateProductDto dto)
    {
        product.ProductName = dto.Name;
        product.SKU = dto.Sku;
        product.Category = dto.Category;
        product.Price = dto.Price;
        product.StockQuantity = dto.Stock;
        product.Status = dto.Status;
        product.UpdatedAt = DateTime.UtcNow;
    }

    public static List<ProductDto> ToDtoList(IEnumerable<Product> products)
    {
        return products.Select(ToDto).ToList();
    }
}
