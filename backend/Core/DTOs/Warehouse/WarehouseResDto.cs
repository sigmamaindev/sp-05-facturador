namespace Core.DTOs.Warehouse;

public class WarehouseResDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsMain { get; set; }
}
