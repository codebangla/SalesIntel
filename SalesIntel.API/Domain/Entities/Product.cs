using System.ComponentModel.DataAnnotations;

namespace SalesIntel.API.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int StockQuantity { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Inventory> InventoryRecords { get; set; } = new List<Inventory>();
}
