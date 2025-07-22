using SalesIntel.API.Domain.Entities;

namespace SalesIntel.API.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(SalesIntelDbContext context)
    {
        if (context.Products.Any())
            return; // Already seeded

        // Seed Products
        var products = new List<Product>
        {
            new Product
            {
                ProductName = "Wireless Headphones",
                SKU = "SKU-001",
                Category = "Electronics",
                Price = 99.99m,
                StockQuantity = 150,
                Status = "Active"
            },
            new Product
            {
                ProductName = "Smart Watch",
                SKU = "SKU-002",
                Category = "Electronics",
                Price = 299.99m,
                StockQuantity = 75,
                Status = "Active"
            },
            new Product
            {
                ProductName = "Coffee Maker",
                SKU = "SKU-003",
                Category = "Appliances",
                Price = 149.99m,
                StockQuantity = 25,
                Status = "Active"
            },
            new Product
            {
                ProductName = "Desk Lamp",
                SKU = "SKU-004",
                Category = "Furniture",
                Price = 79.99m,
                StockQuantity = 0,
                Status = "Inactive"
            },
            new Product
            {
                ProductName = "Bluetooth Speaker",
                SKU = "SKU-005",
                Category = "Electronics",
                Price = 59.99m,
                StockQuantity = 5,
                Status = "Active"
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Seed Orders
        var orders = new List<Order>
        {
            new Order
            {
                OrderNumber = "ORD-20240115-001",
                CustomerName = "Acme Corp",
                OrderType = "Invoice",
                Status = "Completed",
                OrderDate = DateTime.UtcNow.AddDays(-5),
                TotalAmount = 2500.00m
            },
            new Order
            {
                OrderNumber = "ORD-20240114-002",
                CustomerName = "TechStart Inc",
                OrderType = "Order",
                Status = "Processing",
                OrderDate = DateTime.UtcNow.AddDays(-6),
                TotalAmount = 1200.00m
            },
            new Order
            {
                OrderNumber = "QUO-20240113-003",
                CustomerName = "Global Solutions",
                OrderType = "Quotation",
                Status = "Pending",
                OrderDate = DateTime.UtcNow.AddDays(-7),
                TotalAmount = 3800.00m
            }
        };

        context.Orders.AddRange(orders);
        await context.SaveChangesAsync();

        // Seed Order Items
        var orderItems = new List<OrderItem>
        {
            new OrderItem
            {
                OrderId = orders[0].Id,
                ProductId = products[0].Id,
                Quantity = 10,
                UnitPrice = 99.99m,
                TotalPrice = 999.90m
            },
            new OrderItem
            {
                OrderId = orders[0].Id,
                ProductId = products[1].Id,
                Quantity = 5,
                UnitPrice = 299.99m,
                TotalPrice = 1499.95m
            },
            new OrderItem
            {
                OrderId = orders[1].Id,
                ProductId = products[2].Id,
                Quantity = 8,
                UnitPrice = 149.99m,
                TotalPrice = 1199.92m
            }
        };

        context.OrderItems.AddRange(orderItems);
        await context.SaveChangesAsync();

        // Seed Inventory
        var inventoryRecords = new List<Inventory>
        {
            new Inventory
            {
                ProductId = products[0].Id,
                CurrentStock = 150,
                ReorderLevel = 50,
                MaxStock = 300,
                Location = "Warehouse A"
            },
            new Inventory
            {
                ProductId = products[1].Id,
                CurrentStock = 75,
                ReorderLevel = 25,
                MaxStock = 150,
                Location = "Warehouse A"
            },
            new Inventory
            {
                ProductId = products[2].Id,
                CurrentStock = 25,
                ReorderLevel = 10,
                MaxStock = 100,
                Location = "Warehouse B"
            }
        };

        context.Inventory.AddRange(inventoryRecords);
        await context.SaveChangesAsync();
    }
}
