namespace Core.Entities;

public class Supplier : BaseEntity
{
    public string BusinessName { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = [];
}
