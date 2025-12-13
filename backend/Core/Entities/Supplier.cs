namespace Core.Entities;

public class Supplier : BaseEntity
{
    public required string BusinessName { get; set; }
    public required string Document { get; set; }
    public required string Address { get; set; }
    public required string Email { get; set; }
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = [];
}
