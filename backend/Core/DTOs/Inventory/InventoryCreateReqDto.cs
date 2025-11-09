namespace Core.DTOs.Inventory;

public class InventoryCreateReqDto
{
    public List<InventoryItemReqDto> Inventories { get; set; } = [];
}

public class InventoryItemReqDto
{
    public decimal Stock { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public int WarehouseId { get; set; }
}
