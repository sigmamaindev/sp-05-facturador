namespace Core.Entities;

public class Business : BaseEntity
{
    public required string Document { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserBusiness> UserBusiness { get; set; } = [];
    public ICollection<Warehouse> Warehouses { get; set; } = [];
    public ICollection<Establishment> Establishments { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<UnitMeasure> UnitMeasures { get; set; } = [];
    public ICollection<Tax> Taxes { get; set; } = [];
    public ICollection<Customer> Customers { get; set; } = [];
}
