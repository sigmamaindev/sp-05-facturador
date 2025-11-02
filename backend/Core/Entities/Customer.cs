namespace Core.Entities;

public class Customer : BaseEntity
{
    public required string Document { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
}
