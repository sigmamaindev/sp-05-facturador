namespace Core.DTOs.Inventory;

public class InventoryResDto
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public int ProductId { get; set; }
}
