using SalesIntel.API.Domain.Entities;
using SalesIntel.API.Application.DTOs;

namespace SalesIntel.API.Application.Transformers;

public static class OrderTransformer
{
    public static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Customer = order.CustomerName,
            Type = order.OrderType,
            Amount = order.TotalAmount,
            Status = order.Status,
            Date = order.OrderDate,
            Items = order.OrderItems.Count,
            OrderItems = order.OrderItems.Select(ToItemDto).ToList()
        };
    }

    public static OrderItemDto ToItemDto(OrderItem item)
    {
        return new OrderItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.ProductName ?? string.Empty,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        };
    }

    public static Order ToEntity(CreateOrderDto dto)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerName = dto.Customer,
            OrderType = dto.Type,
            Status = "Pending",
            OrderDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var itemDto in dto.Items)
        {
            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = itemDto.Quantity * itemDto.UnitPrice
            };
            order.OrderItems.Add(orderItem);
        }

        order.TotalAmount = order.OrderItems.Sum(x => x.TotalPrice);
        return order;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
    }

    public static List<OrderDto> ToDtoList(IEnumerable<Order> orders)
    {
        return orders.Select(ToDto).ToList();
    }
}
