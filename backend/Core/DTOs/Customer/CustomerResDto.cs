using Core.DTOs.Business;
using Core.DTOs.DocumentType;

namespace Core.DTOs.Customer;

public class CustomerResDto
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
