namespace Core.DTOs.Warehouse;

public class EditWarehouseReqDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int BusinessId { get; set; }
}
