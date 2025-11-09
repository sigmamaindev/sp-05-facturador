namespace Core.DTOs.Inventory;

public class InventoryUpdateReqDto
{
    public decimal Stock { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public int WarehouseId { get; set; }
}
