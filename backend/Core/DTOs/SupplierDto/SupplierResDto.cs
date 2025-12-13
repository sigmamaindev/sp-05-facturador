using System;

namespace Core.DTOs.SupplierDto;

public class SupplierResDto
{
    public int Id { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

}
