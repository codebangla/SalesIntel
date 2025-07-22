using System.ComponentModel.DataAnnotations;

namespace SalesIntel.API.Domain.Entities;

public class Inventory
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public int CurrentStock { get; set; }
    
    public int ReorderLevel { get; set; }
    
    public int MaxStock { get; set; }
    
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
