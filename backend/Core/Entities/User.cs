namespace Core.Entities;

public class User : BaseEntity
{
    public required string Document { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public string? ImageUrl { get; set; }
    public string? CompanyName { get; set; }
    public string? Sequence { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserRole> UserRole { get; set; } = [];
    public ICollection<UserBusiness> UserBusiness { get; set; } = [];
    public ICollection<UserEstablishment> UserEstablishment { get; set; } = [];
    public ICollection<UserEmissionPoint> UserEmissionPoint { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
