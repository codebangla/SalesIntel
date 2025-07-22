using System.ComponentModel.DataAnnotations;

namespace SalesIntel.API.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string OrderType { get; set; } = "Quotation"; // Quotation, Order, Invoice
    
    public decimal TotalAmount { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
}
