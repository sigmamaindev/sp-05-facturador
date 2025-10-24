namespace Core.DTOs.Warehouse;

public class GetWarehouseResDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public GetWarehouseBusinessDto? Business{ get; set; }
}
